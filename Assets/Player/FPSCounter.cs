using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textMesh;
    private const float fpsAveragingDur = 0.1f;
    private readonly List<(float clockTime, float deltaTime)> times = new();

    private void LateUpdate() {

        // fps is calculated by the average deltaTime over the past fpsAveragingDur seconds

        times.Add((Time.unscaledTime, Time.unscaledDeltaTime));
        times.RemoveAll(time => Time.unscaledTime - time.clockTime > fpsAveragingDur);

        float sum = 0f;
        foreach (var t in times) sum += t.deltaTime;
        float fps = Mathf.Round(times.Count / sum);

        textMesh.text = fps.ToString();
    }
}
