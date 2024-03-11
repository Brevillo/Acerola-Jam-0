using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverBeebe.UnityUtilities.Runtime;

[CreateAssetMenu]
public class BulletPool : ScriptableObject {

    [SerializeField] private Material bulletMaterial;
    [SerializeField] private Mesh bulletMesh;
    [SerializeField, Readonly] private int count;

    public static float timeScale = 1;
    public static float deltaTime => timeScale * Time.deltaTime;

    [System.Serializable]
    public struct BulletParameters {

        public float damage;
        public float radius;
        public float lifetime;

        public bool followGround;

        public AnimationCurve sizeOverLifeTime;
    }

    public struct Bullet {

        public Vector3 position;
        public Vector3 initialVelocity;
        public Vector3 velocity;
        public float size;
        public float lifetime;
        
        public Bullet(Vector3 position, Vector3 velocity, float lifetime) {
            this.position = position;
            this.initialVelocity = velocity;
            this.velocity = velocity;
            this.lifetime = lifetime;
            this.size = default;
        }
    }

    public class BulletRegister {

        public BulletRegister(BulletParameters parameters) {
            this.parameters = parameters;
        }

        public List<Bullet> bullets = new();
        private BulletParameters parameters;

        public void Spawn(Vector3 position, Vector3 velocity) => bullets.Add(new(position, velocity, parameters.lifetime));

        public void Update(float deltaTime) {

            for (int i = 0; i < bullets.Count; i++) {

                var bullet = bullets[i];

                if (parameters.followGround && Physics.Raycast(bullet.position, Vector3.down, out var hit, Mathf.Infinity, GameInfo.GroundMask))
                    bullet.velocity = Quaternion.FromToRotation(Vector3.up, hit.normal) * bullet.initialVelocity;

                bullet.position += bullet.velocity * deltaTime;
                bullet.lifetime -= deltaTime;
                bullet.size = parameters.sizeOverLifeTime.Evaluate(1 - bullets[i].lifetime / parameters.lifetime);

                bullets[i] = bullet;

                if (bullet.lifetime <= 0) {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }

        public void CheckCollisions(Player player) {

            Bounds bounds = player.Bounds;
            Vector3 size = Vector3.one * parameters.radius;

            foreach (var bullet in bullets.FindAll(bullet => bounds.Intersects(new(bullet.position, size)))) {

                player.TakeDamage(new() {
                    damage = parameters.damage
                });

                bullets.Remove(bullet);
            }
        }

        public void Prewarm(float duration, float deltaTime, System.Action<float> updateFunction) {

            for (float time = 0; time < duration; time += deltaTime) {
                updateFunction.Invoke(deltaTime);
                Update(deltaTime);
            }
        }
    }

    private List<BulletRegister> registers = new();

    public BulletRegister Register(BulletParameters parameters) {

        var register = new BulletRegister(parameters);

        registers.Add(register);

        return register;
    }

    public void Deregister(BulletRegister register) {
        registers.Remove(register);
    }

    public void UpdateBullets(Player player, float deltaTime) {

        deltaTime *= timeScale;

        foreach (var register in registers) {
            register.Update(deltaTime);
            register.CheckCollisions(player);
        }
    }

    public void RenderBullets(Player player) {

        List<Bullet> bullets = new();

        foreach (var register in registers)
            bullets.AddRange(register.bullets);

        count = bullets.Count;

        // sort by distance to camera

        Vector3 camera = player.Camera.transform.position;
        int Distance(Bullet b1, Bullet b2) => (b2.position - camera).sqrMagnitude.CompareTo((b1.position - camera).sqrMagnitude);
        bullets.Sort(Distance);

        // generate bullet position/rotation matrices

        var instanceData = new Matrix4x4[bullets.Count];
        var cameraRotation = player.Camera.transform.rotation;
        for (int i = 0; i < bullets.Count; i++)
            instanceData[i] = Matrix4x4.TRS(bullets[i].position, cameraRotation, Vector3.one * bullets[i].size);

        // render bullets

        Graphics.RenderMeshInstanced(new(bulletMaterial), bulletMesh, 0, instanceData);
    }
}
