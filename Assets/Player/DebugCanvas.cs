using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugCanvas : Player.Component {

    [SerializeField] private GameObject content;

    private void Update() {
        ShowCanvas(Input.Debug.Pressed);
    }

    private void ShowCanvas(bool active) {
        content.SetActive(active);
        ShowCursor = active;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleDebugTrail() {
        TrailManager.showDebugTrail = !TrailManager.showDebugTrail;
    }

    public void ToggleGodemode() {
        PlayerMovement.godmode = !PlayerMovement.godmode;
    }

    public void ToggleTrailEffects() {
        TrailManager.trailEffectsActive = !TrailManager.trailEffectsActive;
    }
}
