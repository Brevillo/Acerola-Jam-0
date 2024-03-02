using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerDamageInfo {
    public float damage;
}

public class PlayerHealth : Player.Component {

    [SerializeField] private float maxHealth;
    [SerializeField] private float invincibiltyDuration;
    [SerializeField] private float health;
    [SerializeField] private Image healthbar;

    private readonly Collider[] bulletColliders = new Collider[1];

    private float invincibilityRemaining;

    private void Start() {
        health = maxHealth;
    }

    private void Update() {

        invincibilityRemaining -= Time.deltaTime;

        Vector3 center = Collider.transform.TransformPoint(Collider.center),
                capsuleTop = center + Vector3.up * Collider.height / 2f,
                capsuleBottom = center + Vector3.down * Collider.height / 2f;

        if (Physics.OverlapCapsuleNonAlloc(capsuleBottom, capsuleTop, Collider.radius, bulletColliders, GameInfo.BulletMask) > 0)
            TakeDamage(new() {
                damage = bulletColliders[0].GetComponent<IBullet>().Damage
            });

        healthbar.fillAmount = health / maxHealth;
    }

    public void TakeDamage(PlayerDamageInfo info) {

        if (invincibilityRemaining > 0) return;

        health = Mathf.MoveTowards(health, 0, info.damage);

        invincibilityRemaining = invincibiltyDuration;
    }
}
