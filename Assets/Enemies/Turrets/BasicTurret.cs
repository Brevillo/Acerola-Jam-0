using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private BulletSpawner spawner;

    private float fireTimer;
    private Transform target;

    private void Start() {
        target = FindObjectOfType<Player>().transform;
        spawner.Prewarm(AttackUpdate);
    }

    private void Update() {
        AttackUpdate(BulletPool.deltaTime);
    }

    private void AttackUpdate(float deltaTime) {

        fireTimer += deltaTime;

        if (fireTimer > fireRate) {
            fireTimer = 0;
            spawner.Spawn(bulletOrigin.position, (target.position - bulletOrigin.position).normalized * fireSpeed);
        }
    }
}
