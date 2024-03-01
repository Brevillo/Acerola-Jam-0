using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private Rigidbody bulletPrefab;

    private float fireTimer;
    private Transform target;

    private void Start() {
        target = FindObjectOfType<Player>().transform;
    }

    private void Update() {

        fireTimer += Time.deltaTime;

        if (fireTimer > fireRate) {
            fireTimer = 0;
            Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity).velocity = (target.position - bulletOrigin.position).normalized * fireSpeed;
        }
    }
}
