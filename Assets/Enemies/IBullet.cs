using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBullet : MonoBehaviour {

    [SerializeField] private new Rigidbody rigidbody;

    protected void Destroy() => OnDeactivate?.Invoke(this);

    public event System.Action<IBullet> OnDeactivate;

    public Vector3 Velocity { set => rigidbody.velocity = value; }
    public Vector3 Position { set => transform.position = value; }
}
