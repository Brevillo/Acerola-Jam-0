using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : IBullet {

    [SerializeField] private float damage;
    [SerializeField] private float lifetime;

    private float lifetimer;

    private void Update() {

        lifetimer += Time.deltaTime;

        if (lifetimer > lifetime) Destroy();
    }

    private void OnTriggerEnter(Collider other) {

        if (other.TryGetComponent(out Player player)) {

            player.TakeDamage(new() {
                damage = damage
            });

            Destroy();
        }

        else if (other.gameObject.layer == GameInfo.GroundLayer)
            Destroy();
    }
}
