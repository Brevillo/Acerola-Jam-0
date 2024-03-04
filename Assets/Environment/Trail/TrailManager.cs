using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using OliverBeebe.UnityUtilities.Runtime;
using OliverUtils;
using UnityEditor.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrailManager : MonoBehaviour {

    [SerializeField] private TrailMember trailHead;
    [SerializeField] private SubTrail trail;
    [SerializeField] private float trailFadeWidth;
    [SerializeField] private AnimationCurve trailFadeCurve;
    [SerializeField] private float minFog, maxFog;
    [SerializeField] private float maxChromaticAbberation;
    [SerializeField] private VolumeProfile postProcessing;

    [Header("Debug Line Renderers")]
    [SerializeField] private float debugLineHeight;
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float memberGizmoRadius;
    [SerializeField] private Material debugLineMaterial;
    [SerializeField] private Color innerDebugLineColor, outerDebugLineColor;

    private Transform target;

    public static bool showDebugTrail;

    [System.Serializable]
    private class SubTrail {

        public TrailMember head;
        public List<SubTrail> subTrails;

        public bool DebugTrailEnabled {
            set {
                if (debugLinesParent != null) debugLinesParent.SetActive(value);
                if (subTrails != null) foreach (var trail in subTrails) trail.DebugTrailEnabled = value;
            }
        }
        public GameObject debugLinesParent;
        public LineRenderer outerDebugLine, innerDebugLine;

        public SubTrail(TrailManager manager, TrailMember head) {

            this.head = head;
            head.AddManager(manager);

            subTrails = new();
            foreach (Transform child in head.transform)
                if (child.TryGetComponent(out TrailMember member))
                    subTrails.Add(new SubTrail(manager, member));

            GenerateDebugLines();
        }

        public List<Vector3> Positions {
            get {
                var positions = subTrails.ConvertAll(trail => trail.head.transform.position);
                positions.Insert(0, head.transform.position);
                return positions;
            }
        }

        public void Cleanup() {

            if (debugLinesParent != null) DestroyImmediate(debugLinesParent);
            if (outerDebugLine != null) DestroyImmediate(outerDebugLine.gameObject);
            if (innerDebugLine != null) DestroyImmediate(innerDebugLine.gameObject);
            if (subTrails != null) foreach (var member in subTrails) member.Cleanup();

            subTrails = null;
        }

        public void DrawGizmos(TrailManager manager) {

            if (subTrails == null || subTrails.Count == 0) return;

            // remove deleted members to avoid errors
            subTrails.RemoveAll(trail => trail.head == null);

            // recurse
            foreach (var trail in subTrails) trail.DrawGizmos(manager);

            // get subtrail head positions
            Gizmos.DrawLineStrip(new(Positions.ToArray()), false);

            // debug line renderers
            UpdateDebugLines(manager);
        }

        #region Debug Lines

        public void GenerateDebugLines() {

            if (subTrails.Count == 0) return;

            debugLinesParent = new GameObject("Debug Lines");
            debugLinesParent.transform.parent = head.transform;

            outerDebugLine = GenerateLine("Outer Debug Line", 1);
            innerDebugLine = GenerateLine("Inner Debug Line", 2);

            LineRenderer GenerateLine(string name, int orderInLayer) {

                var line = new GameObject(name, typeof(LineRenderer)).GetComponent<LineRenderer>();

                line.transform.parent = debugLinesParent.transform;
                line.transform.localPosition = Vector3.zero;
                line.transform.localEulerAngles = Vector3.right * 90f;

                line.useWorldSpace = true;
                line.alignment = LineAlignment.TransformZ;
                line.numCapVertices = 10;
                line.numCornerVertices = 10;
                line.sortingOrder = orderInLayer;

                return line;
            }
        }

        public void UpdateDebugLines(TrailManager manager) {

            // raycast down to get ground points
            var linePoints = Positions
                .ConvertAll(point => Physics.Raycast(point, Vector3.down, out var hit, Mathf.Infinity, GameInfo.GroundMask)
                    ? hit.point + Vector3.up * manager.debugLineHeight
                    : point)
                .ToArray();

            UpdateLine(outerDebugLine, 0, manager.outerDebugLineColor);
            UpdateLine(innerDebugLine, manager.trailFadeWidth, manager.innerDebugLineColor);

            void UpdateLine(LineRenderer line, float widthSub, Color color) {

                if (line == null) return;

                line.material = manager.debugLineMaterial;
                line.colorGradient = new() {
                    colorKeys = new[] { new GradientColorKey(color, 0) },
                    alphaKeys = new[] { new GradientAlphaKey(color.a, 0) },
                };

                line.positionCount = linePoints.Length;
                line.SetPositions(linePoints);

                var frames = new Keyframe[subTrails.Count];
                for (int i = 0; i < subTrails.Count; i++)
                    frames[i] = new((float)i / subTrails.Count, subTrails[i].head.Width - widthSub);
                line.widthCurve = new(frames);
            }
        }

        #endregion
    }

    private void Start() {
        target = FindObjectOfType<Player>().transform;
        ConstructTrail();
    }

    private void Update() {

        trail.DebugTrailEnabled = showDebugTrail;

        float trailPercent = CalculatePlayerTrailPercent();

        RenderSettings.fogDensity = Mathf.Lerp(minFog, maxFog, trailPercent);

        if (postProcessing.TryGet(out ChromaticAberration chromaticAberration))
            chromaticAberration.intensity.value = Mathf.Lerp(0, maxChromaticAbberation, trailPercent);
    }

    private float CalculatePlayerTrailPercent() {

        Vector2 player = new(target.position.x, target.position.z);

        (TrailMember m1, TrailMember m2) closestSegment = default;
        float smallestDist = Mathf.Infinity;

        static Vector2 Position(TrailMember member) => new(member.transform.position.x, member.transform.position.z);

        CheckForClosestSegment(trail);
        void CheckForClosestSegment(SubTrail trail) {

            var positions = trail.Positions.ConvertAll(position => new Vector2(position.x, position.z));

            TrailMember GetMember(int i) => i == 0 ? trail.head : trail.subTrails[i - 1].head;

            for (int i = 0; i < positions.Count - 1; i++) {

                float dist = Util.DistToLine(player, positions[i], positions[i + 1]).magnitude;

                if (dist < smallestDist) {
                    smallestDist = dist;
                    closestSegment = (GetMember(i), GetMember(i + 1));
                }
            }

            // check sub trails
            foreach (var subTrail in trail.subTrails)
            CheckForClosestSegment(subTrail);
        }

        (TrailMember member1, TrailMember member2) = closestSegment;
        Vector2 position1 = Position(member1),
                position2 = Position(member2);

        Debug.DrawRay(member1.transform.position, Vector3.up * 5, Color.green);
        Debug.DrawRay(member2.transform.position, Vector3.up * 5, Color.green);

        float linePerecnt = Util.InverseLerpFromClosestPoint(player, position1, position2),
              lineWidth   = Mathf.Lerp(member1.Width, member2.Width, linePerecnt),
              distToLine  = Util.DistToLine(player, position1, position2).magnitude,
              fadePercent = 1 - trailFadeCurve.Evaluate((lineWidth / 2f - distToLine) / trailFadeWidth);

        return fadePercent;
    }

    private void DestroyTrail() {
        trail.Cleanup();
    }

    private void ConstructTrail() {
        DestroyTrail();
        trail = new(this, trailHead);
    }

    #region Editor

    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        trail.DrawGizmos(this);
    }

    public void DrawMemberGizmo(TrailMember member) {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(member.transform.position, memberGizmoRadius);
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(TrailManager))]
    private class TrailManagerEditor : Editor {

        private TrailManager manager => target as TrailManager;

        public override void OnInspectorGUI() {

            base.OnInspectorGUI();

            if (GUILayout.Button("Construct Trail")) manager.ConstructTrail();
            if (GUILayout.Button("Destroy Trail")) manager.DestroyTrail();
        }
    }

    #endif

    #endregion
}
