using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu]
public class BulletPool : ScriptableObject {

    [SerializeField] private IBullet bulletPrefab;

    private ObjectPool<IBullet> pool;
    private Transform heirarchyFolder;

    public IBullet Spawn(Vector3 position) {

        if (pool == null) Initialize();

        var bullet = pool.Get();

        bullet.Position = position;
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void Destroy(IBullet bullet) => pool.Release(bullet);

    private void Initialize() {

        pool = new(
            createFunc:         OnPoolCreate,
            actionOnRelease:    OnPoolRelease,
            actionOnDestroy:    OnPoolDestroy);

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (s1, s2) => pool.Clear();

        heirarchyFolder = new GameObject(name).transform;
    }

    private IBullet OnPoolCreate() {

        var bullet = Instantiate(bulletPrefab, heirarchyFolder).GetComponent<IBullet>();
        bullet.OnDeactivate += pool.Release;

        return bullet;
    }

    private void OnPoolRelease(IBullet bullet) {
        bullet.gameObject.SetActive(false);
    }

    private void OnPoolDestroy(IBullet bullet) {
        Destroy(bullet.gameObject);
    }
}
