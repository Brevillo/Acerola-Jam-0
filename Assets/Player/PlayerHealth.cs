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

    [Header("Health Bar")]
    [SerializeField] private Image healthbar;
    [SerializeField] private Image lazybar;
    [SerializeField] private float lazybarDelay, lazybarSpeed;
    [SerializeField] private SmartCurve healthShakeIntensity;
    [SerializeField] private float healthShakeSpeed;
    [SerializeField] private List<RectTransform> healthShakingTransforms;

    [Header("Taking Damage")]
    [SerializeField] private float damageTimeStop;
    [SerializeField] private SmartCurve damageFlashCurve;
    [SerializeField] private CanvasGroup damageFlashUI;

    [Header("Death")]
    [SerializeField] private SmartCurve deathBulletSlowdown;
    [SerializeField] private GameObject reaperPrefab;
    [SerializeField] private Image reaperTouch;
    [SerializeField] private float reaperDeathDelay;

    private List<UIShake> uiShakes;

    private class UIShake {

        public UIShake(RectTransform transform, PlayerHealth health) {
            this.transform = transform;
            this.health = health;
        }

        public void Update() {

            transform.anchoredPosition = health.dying
                ? Vector2.zero
                : Vector2.MoveTowards(transform.anchoredPosition, healthShakeTarget, health.healthShakeSpeed * Time.deltaTime);

            if (transform.anchoredPosition == healthShakeTarget)
                healthShakeTarget = Random.insideUnitCircle * health.healthShakeIntensity.EvaluateAt(health.health / health.maxHealth);
        }

        public void Stop() {
            transform.anchoredPosition = Vector2.zero;
        }

        private readonly PlayerHealth health;
        private readonly RectTransform transform;
        private Vector2 healthShakeTarget;
    }

    private float invincibilityRemaining;
    private float timeSinceHit;
    private Coroutine damageFlash;
    private bool dying;


    private void Start() {

        health = maxHealth;

        reaperTouch.enabled = false;
        damageFlashUI.alpha = 0;

        BulletPool.timeScale = 1;

        uiShakes = healthShakingTransforms.ConvertAll(transform => new UIShake(transform, this));
    }

    private void Update() {

        invincibilityRemaining -= Time.deltaTime;

        // health bar

        healthbar.fillAmount = health / maxHealth;

        timeSinceHit += Time.deltaTime;
        if (timeSinceHit > lazybarDelay) {
            lazybar.fillAmount = Mathf.Max(healthbar.fillAmount, Mathf.MoveTowards(lazybar.fillAmount, healthbar.fillAmount, lazybarSpeed * Time.deltaTime));
        }

        foreach (var shake in uiShakes)
            shake.Update();
    }

    public void TakeDamage(PlayerDamageInfo info) {

        if (invincibilityRemaining > 0 || dying) return;

        health = Mathf.MoveTowards(health, 0, info.damage);

        timeSinceHit = 0;

        invincibilityRemaining = invincibiltyDuration;

        if (damageFlash != null) StopCoroutine(damageFlash);
        damageFlash = StartCoroutine(HitFlash());

        if (health == 0) Death();

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

    private void Death() {

        dying = true;

        foreach (var shake in uiShakes)
            shake.Stop();

        StartCoroutine(TimeSlow());
        IEnumerator TimeSlow() {

            deathBulletSlowdown.Start();
            while (!deathBulletSlowdown.Done) {
                BulletPool.timeScale = deathBulletSlowdown.Evaluate();
                yield return null;
            }

            Instantiate(reaperPrefab, transform.position, Quaternion.identity);
        }
    }

    public void ReaperTouch() {

        StartCoroutine(DeathDelay());
        IEnumerator DeathDelay() {

            reaperTouch.enabled = true;

            yield return new WaitForSeconds(reaperDeathDelay);

            BulletPool.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
