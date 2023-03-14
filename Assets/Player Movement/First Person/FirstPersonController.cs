using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(Stamina))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float crouchWalkSpeed = 1.5f;
    [SerializeField] private float speedChangeSmoothTime = 0.2f;

    [Header("Jump and Gravity")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float crouchJumpHeight = 1f;
    [SerializeField] private float jumpDelayAfterLanding = 0.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private float groundRadius = 0.5f;
    [SerializeField] private float groundSpherecastDistance = 0.7f;

    [Header("Crouch")]
    [SerializeField] private LayerMask crouchLayer;
    [SerializeField] private float crouchZoneOffset = 1f;
    [SerializeField] private float crouchRadius = 0.5f;
    [SerializeField] private float crouchSmoothTime = 0.3f;

    private PlayerInputHandler inputHandler;
    private HeadBobController headBob;
    private Stamina stamina;
    private Vector3 originalControllerCenter;
    private Vector3 originalCameraPosition;
    private Vector3 crouchCenterVelocity;
    private Vector3 cameraCrouchVelocity;
    private float crouchHeightVelocity;
    private float originalControllerHeight;
    private float verticalCameraAngle;
    private float jumpVelocity;
    private float jumpDelayTimer;
    private float velocity;

    public CharacterController Controller { get; private set; }
    public Vector3 Move { get; private set; }
    public float HorizontalSpeed { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsCrouched { get; private set; }
    public bool WasPreviouslyGrounded { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        Controller = GetComponent<CharacterController>();
        headBob = GetComponentInChildren<HeadBobController>();
        stamina = GetComponent<Stamina>();

        originalControllerHeight = Controller.height;
        originalControllerCenter = Controller.center;
        originalCameraPosition = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        MovementAndLook();
        JumpAndGravity();
        CrouchCheck();
    }

    void MovementAndLook() {
        // Rotations
        transform.Rotate(Vector3.up * inputHandler.GetLookInputHorizontal());

        verticalCameraAngle += inputHandler.GetLookInputVertical();
        verticalCameraAngle = Mathf.Clamp(verticalCameraAngle, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalCameraAngle, 0f, 0f);

        // Movement
        Move = inputHandler.GetMoveInput();

        Vector3 relativeControllerDirection = transform.InverseTransformVector(Controller.velocity);

        IsRunning = IsGrounded() && inputHandler.GetRunInput() && relativeControllerDirection.z > 0.1f && !IsInCrouchZone();

        if (!stamina.CanRun) IsRunning = false;

        Move = transform.TransformVector(Move); // Move relative to transform

        float targetSpeed = IsRunning ? runSpeed : IsCrouched ? crouchWalkSpeed : walkSpeed;

        if (IsRunning) IsCrouched = false;

        if (Move.sqrMagnitude < 0.01f) targetSpeed = 0f;

        CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, targetSpeed, ref velocity, speedChangeSmoothTime);
        CurrentSpeed = Mathf.Round(CurrentSpeed * 1000) / 1000; // Round by 3 decimals

        Controller.Move((Time.deltaTime * CurrentSpeed * Move) + new Vector3(0, jumpVelocity, 0) * Time.deltaTime);

        HorizontalSpeed = new Vector3(Controller.velocity.x, 0f, Controller.velocity.z).sqrMagnitude;
    }

    void JumpAndGravity() {
        if (IsGrounded()) {
            if (jumpVelocity < 0f)
                jumpVelocity = -2f;

            if (jumpDelayTimer >= 0f)
                jumpDelayTimer -= Time.deltaTime;

            if (inputHandler.GetJumpInput() && jumpDelayTimer <= 0f) {
                if (IsInCrouchZone()) return;

                float targetJumpHeight = IsCrouched ? crouchJumpHeight : jumpHeight;

                jumpVelocity += Mathf.Sqrt(targetJumpHeight * -2f * gravity); //Was -3f last time, not sure why is it -2f now
                IsCrouched = false;

                //if (footsteps)
                //    footsteps.PlayJumpSound();
            }
        } else {
            jumpDelayTimer = jumpDelayAfterLanding;
        }

        jumpVelocity += gravity * Time.deltaTime;

        if (!WasPreviouslyGrounded && IsGrounded()) {
            StartCoroutine(headBob.JumpBob());
        }

        WasPreviouslyGrounded = IsGrounded();
    }

    public bool IsGrounded() {
        Vector3 spherePosition = new(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    public RaycastHit? GetGroundRaycastHit() {
        if (Physics.SphereCast(transform.position + Vector3.up, groundRadius, Vector3.down, out RaycastHit hit, groundSpherecastDistance, groundLayer, QueryTriggerInteraction.Ignore)) {
            return hit;
        } else {
            return null;
        }
    }

    private void CrouchCheck() {
        if (IsGrounded()) {
            if (inputHandler.GetCrouchInput() && !IsInCrouchZone()) {
                IsCrouched = !IsCrouched;
            }
        }

        float targetHeight = IsCrouched ? originalControllerHeight / 2 : originalControllerHeight;
        Vector3 targetCenter = IsCrouched ? originalControllerCenter / 2 : originalControllerCenter;
        Vector3 targetCameraPosition = IsCrouched ? originalCameraPosition / 2 : originalCameraPosition;

        Controller.height = Mathf.SmoothDamp(Controller.height, targetHeight, ref crouchHeightVelocity, crouchSmoothTime);
        Controller.center = Vector3.SmoothDamp(Controller.center, targetCenter, ref crouchCenterVelocity, crouchSmoothTime);
        cameraTransform.localPosition = Vector3.SmoothDamp(cameraTransform.localPosition, targetCameraPosition, ref cameraCrouchVelocity, crouchSmoothTime);
    }

    private bool IsInCrouchZone() {
        Vector3 spherePosition = new(transform.position.x, transform.position.y + crouchZoneOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, crouchRadius, crouchLayer, QueryTriggerInteraction.Ignore) && IsCrouched;
    }

    private void OnDrawGizmosSelected() {
        Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = IsGrounded() ? transparentGreen : transparentRed;

        Vector3 groundPosition = new(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        Gizmos.DrawSphere(groundPosition, groundRadius);

        Gizmos.color = IsInCrouchZone() ? transparentRed : transparentGreen;

        Vector3 crouchPosition = new(transform.position.x, transform.position.y + crouchZoneOffset, transform.position.z);
        Gizmos.DrawSphere(crouchPosition, crouchRadius);

        Gizmos.DrawWireSphere((transform.position + Vector3.up) + Vector3.down * groundSpherecastDistance, groundRadius);
    }
}
