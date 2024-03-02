using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    private new Transform camera;

    private void Awake() {
        camera = Camera.main.transform;
    }

    private void LateUpdate() {
        transform.rotation = camera.rotation;
    }
}
