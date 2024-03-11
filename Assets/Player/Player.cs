using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerInput input;

    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private new Camera camera;

    public Bounds Bounds => collider.bounds;
    public Camera Camera => camera;

    private Dictionary<Component, bool> cursorShow = new();

    public void TakeDamage(PlayerDamageInfo info) => health.TakeDamage(info);

    public class Component : MonoBehaviour {

        [SerializeField] private Player player;

        protected Player Player                 => player;

        protected PlayerMovement    Movement    => player.movement;
        protected PlayerHealth      Health      => player.health;
        protected PlayerInput       Input       => player.input;

        protected Rigidbody         Rigidbody   => player.rigidbody;
        protected CapsuleCollider   Collider    => player.collider;

        protected bool ShowCursor {

            get {
                bool shown = false;
                foreach (var show in player.cursorShow.Values) shown |= show;
                return shown;
            }

            set {

                if (!player.cursorShow.TryAdd(this, value))
                    player.cursorShow[this] = value;

                Cursor.lockState = ShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = ShowCursor;
            }
        }
    }
}
