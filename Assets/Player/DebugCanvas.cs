using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugCanvas : Player.Component {

    [SerializeField] private GameObject content;

    private void Update() {

        bool active = Input.Debug.Pressed;

        content.SetActive(active);
        Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = active;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleDebugTrail() {
        TrailManager.showDebugTrail = !TrailManager.showDebugTrail;
    }
}
