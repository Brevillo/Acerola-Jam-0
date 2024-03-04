using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private BulletPool.BulletParameters bulletParameters;
    [SerializeField] private float prewarmDuration, prewarmDeltaTime;
    [SerializeField] private int bulletsPerAxis;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private BulletPool bulletPool;

    private float fireTimer;
    private float turn;

    private BulletPool.BulletRegister bulletRegister;

    private void Start() {
        bulletRegister = bulletPool.Register(bulletParameters);
        bulletRegister.Prewarm(prewarmDuration, prewarmDeltaTime, AttackUpdate);
    }

    private void OnDestroy() {
        bulletPool.Deregister(bulletRegister);
    }

    private void FixedUpdate() {

        AttackUpdate(Time.fixedDeltaTime);
    }

    private void AttackUpdate(float deltaTime) {

        fireTimer += deltaTime;

        if (fireTimer > fireRate) {

            fireTimer = 0;
            turn += turnSpeed;

            float increment = 360f / bulletsPerAxis;
            float turnAmount = turn % 360;

            for (int y = 0; y < bulletsPerAxis; y++)
                bulletRegister.Spawn(
                    bulletOrigin.position,
                    Quaternion.Euler(0, y * increment + turnAmount, 0) * Vector3.forward * fireSpeed);
        }
    }
}
