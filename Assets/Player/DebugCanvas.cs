using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugCanvas : Player.Component {

    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI velocityTextMesh;

    private void Update() {
        ShowCanvas(Input.Debug.Pressed);
        velocityTextMesh.text = $"{Player.transform.InverseTransformDirection(Rigidbody.velocity)}";
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

    public void Die() {
        Health.TakeDamage(new() {
            damage = int.MaxValue
        });
    }
}
