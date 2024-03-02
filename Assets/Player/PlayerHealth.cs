using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OliverBeebe.UnityUtilities.Runtime;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public struct PlayerDamageInfo {
    public float damage;
}

public class PlayerHealth : Player.Component {

    [SerializeField] private float maxHealth;
    [SerializeField] private float invincibiltyDuration;
    [SerializeField] private float health;
    [SerializeField] private Image healthbar, lazybar;
    [SerializeField] private float lazybarDelay, lazybarSpeed;
    [SerializeField] private float damageTimeStop;
    [SerializeField] private SmartCurve damageFlashCurve;
    [SerializeField] private CanvasGroup damageFlashUI;

    private float invincibilityRemaining;
    private float timeSinceHit;
    private Coroutine damageFlash;

    private void Start() {
        health = maxHealth;
        damageFlashUI.alpha = 0;
    }

    private void Update() {

        invincibilityRemaining -= Time.deltaTime;

        healthbar.fillAmount = health / maxHealth;

        timeSinceHit += Time.deltaTime;

        if (timeSinceHit > lazybarDelay) {
            lazybar.fillAmount = Mathf.Max(healthbar.fillAmount, Mathf.MoveTowards(lazybar.fillAmount, healthbar.fillAmount, lazybarSpeed * Time.deltaTime));
        }
    }

    public void TakeDamage(PlayerDamageInfo info) {

        if (invincibilityRemaining > 0) return;

        health = Mathf.MoveTowards(health, 0, info.damage);

        timeSinceHit = 0;

        invincibilityRemaining = invincibiltyDuration;

        if (damageFlash != null) StopCoroutine(damageFlash);
        damageFlash = StartCoroutine(HitFlash());

        IEnumerator HitFlash() {

            damageFlashUI.alpha = 1;

            yield return TimeManager.FreezeTime(damageTimeStop, this);

            damageFlashCurve.Start();

            while (!damageFlashCurve.Done) {
                float percent = damageFlashCurve.Evaluate();
                damageFlashUI.alpha = percent;
                yield return null;
            }
        }
    }
}
