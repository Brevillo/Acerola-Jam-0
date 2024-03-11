using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverBeebe.UnityUtilities.Runtime;

public class SphereTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private int bulletsPerAxis;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private BulletSpawner spawner;
    [SerializeField] private AudioSource shootSound;

    private float fireTimer;
    private float turn;

    private void Start() {
        spawner.Prewarm(AttackUpdate);
    }

    private void Update() {
        AttackUpdate(BulletPool.deltaTime);
    }

    private void AttackUpdate(float deltaTime) {

        fireTimer += deltaTime;

        if (fireTimer > fireRate) {

            fireTimer = 0;
            turn += turnSpeed;

            float increment = 360f / bulletsPerAxis;
            float turnAmount = turn % 360;

            for (int y = 0; y < bulletsPerAxis; y++)
                spawner.Spawn(
                    bulletOrigin.position,
                    Quaternion.Euler(0, y * increment + turnAmount, 0) * Vector3.forward * fireSpeed);

            shootSound.Play();
        }
    }
}
