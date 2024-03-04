using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour {

    [SerializeField] private List<BulletPool> pools;

    private Player player;

    private void Start() {
        player = FindObjectOfType<Player>();
    }

    private void FixedUpdate() {

        foreach (var pool in pools)
            pool.UpdateBullets(player, Time.fixedDeltaTime);
    }

    private void LateUpdate() {

        foreach (var pool in pools)
            pool.RenderBullets(player);
    }
}
