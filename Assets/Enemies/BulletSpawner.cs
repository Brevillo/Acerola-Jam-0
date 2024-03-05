using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    [SerializeField] private BulletPool.BulletParameters parameters;
    [SerializeField] private float prewarmDuration, prewarmDeltaTime;
    [SerializeField] private BulletPool pool;

    private BulletPool.BulletRegister register;

    public void Spawn(Vector3 position, Vector3 velocity) => register.Spawn(position, velocity);

    public void Prewarm(System.Action<float> updateFunction) {
        register.Prewarm(prewarmDuration, prewarmDeltaTime, updateFunction);
    }

    private void Awake() {
        register = pool.Register(parameters);
    }

    private void OnDestroy() {
        pool.Deregister(register);
    }
}
