using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour {

    [SerializeField] private List<BulletPool> pools;

    private Player player;

    private void Start() {
        player = FindObjectOfType<Player>();

        foreach (var pool in pools)
            pool.Initialize();
    }

    private void Update() {

        foreach (var pool in pools)
            pool.UpdateBullets(player);
    }
}
