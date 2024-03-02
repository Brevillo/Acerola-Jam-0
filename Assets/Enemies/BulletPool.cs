using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BulletPool : ScriptableObject {

    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [SerializeField] private float lifetime;
    [SerializeField] private Material bulletMaterial;
    [SerializeField] private Mesh bulletMesh;
    [SerializeField] private bool sort;

    private struct Bullet {

        public Vector3 position;
        public Vector3 velocity;
        public float lifetime;
        
        public Bullet(Vector3 position, Vector3 velocity, float lifetime) {
            this.position = position;
            this.velocity = velocity;
            this.lifetime = lifetime;
        }
    }

    private List<Bullet> bullets;
    private RenderParams renderParams;

    public void Spawn(Vector3 position, Vector3 velocity) => bullets.Add(new(position, velocity, lifetime));

    public void Initialize() {
        bullets = new();
        renderParams = new(bulletMaterial);
    }

    public void UpdateBullets(Player player) {

        if (bullets.Count == 0) return;

        var playerBounds = player.Bounds;
        Vector3 bulletSize = Vector3.one * radius;
        var cameraRotation = player.Camera.transform.rotation;

        for (int i = 0; i < bullets.Count; i++) {

            var bullet = bullets[i];
            bullet.position += bullet.velocity * Time.deltaTime;
            bullet.lifetime -= Time.deltaTime;
            bullets[i] = bullet;

            if (playerBounds.Intersects(new(bullet.position, bulletSize)))
                player.TakeDamage(new() {
                    damage = damage
                });
        }

        if (sort) {
            Vector3 camera = player.Camera.transform.position;
            int Distance(Bullet b1, Bullet b2) => (int)((b2.position - camera).sqrMagnitude - (b1.position - camera).sqrMagnitude);
            bullets.Sort(Distance);
        }

        var instanceData = new Matrix4x4[bullets.Count];
        for (int i = 0; i < bullets.Count; i++)
            instanceData[i] = Matrix4x4.TRS(bullets[i].position, cameraRotation, Vector3.one);

        static bool Dead(Bullet bullet) => bullet.lifetime <= 0;
        bullets.RemoveAll(Dead);

        Graphics.RenderMeshInstanced(renderParams, bulletMesh, 0, instanceData);
    }
}
