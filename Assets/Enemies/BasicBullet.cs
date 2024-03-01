using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {

    [SerializeField] private float damage;
    [SerializeField] private float lifetime;

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.TryGetComponent(out Player player)) {

            player.TakeDamage(new() {
                damage = damage
            });

            Destroy(gameObject);
        }

        else if (other.gameObject.layer == GameInfo.GroundLayer)
            Destroy(gameObject);
    }
}
