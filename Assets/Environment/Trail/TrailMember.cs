using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMember : MonoBehaviour {

    [SerializeField] private TrailManager manager;
    [SerializeField] private float width = 10;

    public float Width => width;

    public void AddManager(TrailManager manager) => this.manager = manager;

    private void OnDrawGizmos() {

        if (manager != null)
            manager.DrawMemberGizmo(this);
    }
}
