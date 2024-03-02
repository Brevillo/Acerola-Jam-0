using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBullet : MonoBehaviour {

    [SerializeField] private float damage;

    public float Damage => damage;

    public virtual void Spawn(Vector3 position) {
        transform.position = position;
        gameObject.SetActive(true);
    }

    protected virtual void Update() {

        transform.position += Velocity * Time.deltaTime;
    }

    protected void Destroy() => OnDeactivate?.Invoke(this);

    public event System.Action<IBullet> OnDeactivate;

    public Vector3 Velocity;
    public Vector3 Position { set => transform.position = value; }
}
