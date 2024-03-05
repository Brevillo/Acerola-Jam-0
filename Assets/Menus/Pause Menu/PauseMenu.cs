using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Player.Component {

    [SerializeField] private GameObject content;

    private bool paused;

    private void Start() {
        Pause(false);
    }

    private void Update() {

        if (Input.Pause.Down)
            Pause(!paused);
    }

    public void Pause(bool pause) {

        paused = pause;

        if (paused) {
            ShowCursor = true;
            content.SetActive(true);
        } else {
            ShowCursor = false;
            content.SetActive(false);
        }
    }
}
