using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverBeebe.UnityUtilities.Runtime;

public class PlayerMovement : Player.Component {

    [Header("Running")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float groundAccel;
    [SerializeField] private float groundDeccel;
    [SerializeField] private float airAccel;
    [SerializeField] private float airDeccel;
    [SerializeField] private float groundSuckForce;
    [SerializeField] private Wave2 cameraPositionBob;
    [SerializeField] private Wave3 cameraRotationBob;
    [SerializeField] private EffectBlend cameraBobBlend;
    [SerializeField] private Transform cameraEffectsPivot;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpMomentumBoost;
    [SerializeField] private float jumpGravity;
    [SerializeField] private float fallGravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float coyoteTime;
    [SerializeField] private BufferTimer jumpBuffer;
    [SerializeField] private float jumpTiltMultiple, jumpTiltDampSpeed;
    [SerializeField] private EffectBlend jumpTiltBlend;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckOrigin;
    [SerializeField] private float groundCheckRadius;

    [Header("Camera")]
    [SerializeField] private Vector2 lookSpeed;
    [SerializeField] private Transform cameraPivot;

    private StateMachine<PlayerMovement> stateMachine;
    private Grounded grounded;
    private Jumping jumping;
    private Falling falling;

    private Vector3 cameraRotation;
    private bool onGround;
    private bool jumpBuffered;

    private float jumpTilt, jumpTiltVelocity;

    [System.Serializable]
    private class EffectBlend {

        [SerializeField] private float deactivationTime;

        private float strength;
        //private float velocity;

        public static implicit operator float(EffectBlend blend) => blend.strength;

        public float Update(bool active) => strength =
            //Mathf.SmoothDamp(strength, active ? 1 : 0, ref velocity, deactivationTime);
            Mathf.MoveTowards(strength, active ? 1 : 0, 1f / deactivationTime * Time.deltaTime);
    }

    private Vector3 Velocity {
        get => Rigidbody.velocity;
        set => Rigidbody.velocity = value;
    }
    private Vector3 LocalVelocity {
        get => transform.InverseTransformDirection(Velocity);
        set => Velocity = transform.TransformDirection(value);
    }
    private Vector3 SlopeVelocity {
        get => transform.InverseTransformDirection(Quaternion.Inverse(slopeRotation) * Velocity);
        set => Velocity = slopeRotation * transform.TransformDirection(value);
    }

    private Quaternion slopeRotation;

    private readonly Collider[] groundColliders = new Collider[1];

    private void Start() {

        InitStateMachine();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Input.Looking.OnUpdated += OnLookUpdated;

        OnLookUpdated(Vector2.zero);
    }

    private void OnLookUpdated(Vector2 delta) {

        if (this == null || Input.Debug.Pressed) return;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x - delta.y, -90, 90);
        cameraRotation.y += delta.x;

        Rigidbody.MoveRotation(Quaternion.AngleAxis(cameraRotation.y, Vector3.up));
        cameraPivot.localRotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.right);
    }

    private void Update() {

        onGround = Physics.OverlapSphereNonAlloc(groundCheckOrigin.position, groundCheckRadius, groundColliders, GameInfo.GroundMask) > 0;

        slopeRotation = Physics.Raycast(transform.position, Vector3.down, out var slopeHit, Mathf.Infinity, GameInfo.GroundMask)
            ? Quaternion.FromToRotation(Vector3.up, slopeHit.normal)
            : Quaternion.identity;

        jumpBuffered = jumpBuffer.Buffer(Input.Jump.Down);

        stateMachine.Update(Time.deltaTime);

        // camera effects

        cameraEffectsPivot.localPosition = Vector3.zero;
        cameraEffectsPivot.localEulerAngles = Vector3.zero;

        float cameraBobPercent = cameraBobBlend.Update(onGround);

        cameraPositionBob.SetTimer(Time.time);
        cameraRotationBob.SetTimer(Time.time);

        Vector3 velocityPercent = LocalVelocity / runSpeed;
        float movePercent = Mathf.Max(Mathf.Abs(velocityPercent.x), Mathf.Abs(velocityPercent.z)) * cameraBobPercent;
        cameraEffectsPivot.localPosition += (Vector3)cameraPositionBob.position * movePercent;
        cameraEffectsPivot.localEulerAngles += cameraRotationBob.position * movePercent;

        float jumpTiltPercent = jumpTiltBlend.Update(!onGround),
              jumpTiltTarget = Velocity.y * jumpTiltMultiple * jumpTiltPercent;

        jumpTilt = Mathf.SmoothDamp(jumpTilt, jumpTiltTarget, ref jumpTiltVelocity, jumpTiltDampSpeed);
        cameraEffectsPivot.localEulerAngles += Vector3.right * jumpTilt;
    }

    private void Fall(float gravity) {

        Vector3 velocity = Velocity;
        velocity.y = Mathf.MoveTowards(velocity.y, -maxFallSpeed, gravity * Time.deltaTime);
        Velocity = velocity;
    }

    private Vector3 Run(Vector3 velocity, bool keepMomentum) {

        (float accel, float deccel) = onGround
            ? (groundAccel, groundDeccel)
            : (airAccel, airDeccel);

        Vector2 input = Input.Movement.Vector,//.normalized,
                speed = keepMomentum ? new(Mathf.Max(Mathf.Abs(LocalVelocity.x), runSpeed), Mathf.Max(Mathf.Abs(LocalVelocity.z), runSpeed)) : Vector2.one * runSpeed;

        velocity.x = Mathf.MoveTowards(velocity.x, input.x * speed.x, (input.x != 0 ? accel : deccel) * Time.deltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, input.y * speed.y, (input.y != 0 ? accel : deccel) * Time.deltaTime);

        return velocity;
    }

    private void InitStateMachine() {

        grounded = new(this);
        jumping = new(this);
        falling = new(this);

        TransitionDelegate
            toGround    = () => onGround,
            toJump      = () => onGround && jumpBuffered,
            coyoteJump  = () => stateMachine.previousState == grounded && stateMachine.stateDuration < coyoteTime,
            endJump     = () => Velocity.y <= 0,
            toFalling   = () => !onGround;

        stateMachine = new(

            firstState: grounded,

            new() {

                { grounded, new() {
                    new(jumping,    toJump),
                    new(falling,    toFalling),
                } },

                { jumping, new() {
                    new(falling,    endJump),
                } },

                { falling, new() {
                    new(grounded,   toGround),
                    new(jumping,    coyoteJump),
                } },
            }
        );
    }

    private class State : State<PlayerMovement> {
        public State(PlayerMovement context) : base(context) { }
    }

    private class Grounded : State {

        public Grounded(PlayerMovement context) : base(context) { }

        public override void Update() {

            Vector3 slopeVelocity = context.Run(context.SlopeVelocity, false);
            slopeVelocity.y = -context.groundSuckForce;
            context.SlopeVelocity = slopeVelocity;

            base.Update();
        }
    }

    private class Jumping : State {

        public Jumping(PlayerMovement context) : base(context) { }

        public override void Enter() {

            base.Enter();

            Vector3 localVelocity = context.LocalVelocity;

            localVelocity.y = Mathf.Sqrt(context.jumpGravity * context.jumpHeight * 2f);

            Vector2 boost = context.Input.Movement.Vector.normalized * context.jumpMomentumBoost;
            localVelocity += new Vector3(boost.x, 0, boost.y);

            context.LocalVelocity = localVelocity;
        }

        public override void Update() {

            context.LocalVelocity = context.Run(context.LocalVelocity, true);
            context.Fall(context.jumpGravity);

            base.Update();
        }
    }

    private class Falling : State {

        public Falling(PlayerMovement context) : base(context) { }

        public override void Update() {

            context.LocalVelocity = context.Run(context.LocalVelocity, true);
            context.Fall(context.fallGravity);

            base.Update();
        }
    }
}