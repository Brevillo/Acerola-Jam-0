using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerInput input;

    [SerializeField] private new Rigidbody rigidbody;

    public void TakeDamage(PlayerDamageInfo info) => health.TakeDamage(info);

    public class Component : MonoBehaviour {

        [SerializeField] private Player player;

        protected PlayerMovement    Movement    => player.movement;
        protected PlayerHealth      Health      => player.health;
        protected PlayerInput       Input       => player.input;

        protected Rigidbody         Rigidbody   => player.rigidbody;
    }
}
