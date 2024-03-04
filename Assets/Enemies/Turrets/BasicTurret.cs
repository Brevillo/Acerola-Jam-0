using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private BulletPool.BulletParameters parameters;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private BulletPool bulletPool;

    private float fireTimer;
    private Transform target;
    private BulletPool.BulletRegister bulletRegister;

    private void Start() {
        target = FindObjectOfType<Player>().transform;
        bulletRegister = bulletPool.Register(parameters);
    }

    private void OnDestroy() {
        bulletPool.Deregister(bulletRegister);
    }

    private void Update() {

        fireTimer += Time.deltaTime;

        if (fireTimer > fireRate) {
            fireTimer = 0;
            bulletRegister.Spawn(bulletOrigin.position, (target.position - bulletOrigin.position).normalized * fireSpeed);
        }
    }
}
