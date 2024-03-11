using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTurret : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float fireSpeed;
    [SerializeField] private Transform pivot, corner1, corner2;
    [SerializeField] private Vector2 unitsPerBullet;
    [SerializeField] private BulletSpawner spawner;
    [SerializeField] private bool alwaysShowGizmo;

    private float fireTimer;

    private Vector2 size => corner1.InverseTransformPoint(corner2.position);

    private void OnValidate() {

        if (unitsPerBullet.x <= 0) unitsPerBullet.x = 1f;
        if (unitsPerBullet.y <= 0) unitsPerBullet.y = 1f;
    }

    private void Start() {
        spawner.Prewarm(AttackUpdate);
    }

    private void Update() {
        AttackUpdate(Time.deltaTime);
    }

    private void AttackUpdate(float deltaTime) {

        fireTimer += deltaTime;

        if (fireTimer > fireRate) {

            fireTimer = 0;

            for (float x = 0; x < size.x; x += unitsPerBullet.x)
                for (float y = 0; y < size.y; y += unitsPerBullet.y)
                    spawner.Spawn(GetPoint(x, y), pivot.forward * fireSpeed);
        }
    }

    private Vector3 GetPoint(float x, float y) => corner1.TransformPoint(new Vector2(x, y));

    private void OnDrawGizmosSelected() { if (!alwaysShowGizmo) DrawGizmos(); }
    private void OnDrawGizmos() { if (alwaysShowGizmo) DrawGizmos(); }

    private void DrawGizmos() {

        Gizmos.color = Color.red;

        if (unitsPerBullet.x == 0 || unitsPerBullet.y == 0) return;

        for (float x = 0; x < size.x; x += unitsPerBullet.x)
            Gizmos.DrawLine(GetPoint(x, 0), GetPoint(x, size.y));

        for (float y = 0; y < size.y; y += unitsPerBullet.y)
            Gizmos.DrawLine(GetPoint(0, y), GetPoint(size.x, y));
    }
}
