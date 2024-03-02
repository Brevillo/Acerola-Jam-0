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

    [SerializeField] private List<TrailMember> members;
    [SerializeField] private VolumeProfile postProcessingVolume;
    [SerializeField] private float trailFadeWidth;
    [SerializeField] private float minPostExposure, maxPostExposure;
    [SerializeField] private AnimationCurve trailFadeCurve;
    [SerializeField] private float minFog, maxFog;

    [SerializeField] private LineRenderer debugLineInner, debugLineOuter;
    [SerializeField] private float debugLineHeight;
    [SerializeField] private Color gizmoColor;
    [SerializeField] private float memberGizmoRadius;

    private Transform target;

    public static bool showDebugTrail;

    private void Start() {
        target = FindObjectOfType<Player>().transform;   
    }

    private void Update() {

        Vector2 player = new(target.position.x, target.position.z);

        int closestMember = 0;
        float smallestDist = Mathf.Infinity;

        static Vector2 Position(TrailMember member) => new(member.transform.position.x, member.transform.position.z);

        for (int i = 0; i < members.Count - 1; i++) {

            float dist = Util.DistToLine(player, Position(members[i]), Position(members[i + 1])).magnitude;

            if (dist < smallestDist) {
                smallestDist = dist;
                closestMember = i;
            }
        }

        TrailMember member1 = members[closestMember],
                    member2 = members[closestMember + 1];
        Vector2 position1 = Position(member1),
                position2 = Position(member2);

        float linePerecnt = Util.InverseLerpFromClosestPoint(player, position1, position2),
              lineWidth   = Mathf.Lerp(member1.Width, member2.Width, linePerecnt),
              distToLine  = Util.DistToLine(player, position1, position2).magnitude,
              fadePercent = trailFadeCurve.Evaluate((lineWidth / 2f - distToLine) / trailFadeWidth);

        RenderSettings.fogDensity = Mathf.Lerp(minFog, maxFog, fadePercent);

        //if (postProcessingVolume.TryGet(out ColorAdjustments color))
        //    color.postExposure.value = Mathf.Lerp(minPostExposure, maxPostExposure, fadePercent);
    }

    #region Editor

    private void OnDrawGizmos() {

        members = new(GetComponentsInChildren<TrailMember>());
        members.ForEach(member => member.AddManager(this));

        var points = members.ConvertAll(member => member.transform.position);

        Gizmos.color = gizmoColor;
        Gizmos.DrawLineStrip(new(points.ToArray()), false);


        var linePoints = points
            .ConvertAll(point => Physics.Raycast(point, Vector3.down, out var hit, Mathf.Infinity, GameInfo.GroundMask) ? hit.point + Vector3.up * debugLineHeight : point)
            .ToArray();

        if (debugLineInner != null) SetupLine(debugLineInner, trailFadeWidth);
        if (debugLineOuter != null) SetupLine(debugLineOuter, 0);

        void SetupLine(LineRenderer line, float widthSub) {

            line.enabled = showDebugTrail;

            line.positionCount = points.Count;
            line.SetPositions(linePoints);

            var frames = new Keyframe[members.Count];
            for (int i = 0; i < members.Count; i++)
                frames[i] = new((float)i / members.Count, members[i].Width - widthSub);

            line.widthCurve = new(frames);
        }
    }

    public void DrawMemberGizmo(TrailMember member) {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(member.transform.position, memberGizmoRadius);
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(TrailManager))]
    private class TrailManagerEditor : Editor {

        private TrailManager manager => target as TrailManager;
    }

    #endif

    #endregion
}
