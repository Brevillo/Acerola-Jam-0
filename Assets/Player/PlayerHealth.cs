using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerDamageInfo {
    public float damage;
}

public class PlayerHealth : Player.Component {

    [SerializeField] private float maxHealth;
    [SerializeField] private float health;

    private void Start() {
        health = maxHealth;
    }

    public void TakeDamage(PlayerDamageInfo info) {
        health = Mathf.MoveTowards(health, 0, info.damage);
    }
}
