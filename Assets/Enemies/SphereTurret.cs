using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private int bulletsPerAxis;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private BulletPool bulletPool;

    private float fireTimer;

    private void Update() {

        fireTimer += Time.deltaTime;

        if (fireTimer > fireRate) {
            fireTimer = 0;

            float increment = 360f / bulletsPerAxis;

            for (int x = 0; x < bulletsPerAxis; x++)
                for (int y = 0; y < bulletsPerAxis; y++)
                    bulletPool.Spawn(bulletOrigin.position)
                        .Velocity = Quaternion.Euler(x * increment, y * increment, 0)
                            * Vector3.forward * fireSpeed;
        }
    }
}
