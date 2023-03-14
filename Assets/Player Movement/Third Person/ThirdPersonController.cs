using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerInputHandler))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float crouchWalkSpeed = 1.5f;
    [SerializeField] private float speedChangeSmoothTime = 0.2f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float crouchJumpHeight = 1f;
    [SerializeField] private float jumpDelayAfterLanding = 0.2f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private float groundRadius = 0.5f;

    [Header("Crouch")]
    [SerializeField] private LayerMask crouchLayer;
    [SerializeField] private float crouchZoneOffset = 1f;
    [SerializeField] private float crouchRadius = 0.5f;
    [SerializeField] private float crouchSpeedRate = 5f;

    private Rigidbody rb;
    private PlayerInputHandler inputHandler;
    private CapsuleCollider capsule;
    //private Animator anim;
    private Vector3 move;
    private Vector3 originalCapsuleCenter;
    private float originalCapsuleHeight;
    private float rotationVelocity;
    private float currentSpeed;
    private float jumpDelayTimer;
    private float targetJumpHeight;
    private float animatorMovementSpeed;
    private float velocity;
    private bool jump;

    public bool IsRunning { get; private set; }
    public bool IsCrouched { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerInputHandler>();
        capsule = GetComponent<CapsuleCollider>();
        //anim = GetComponent<Animator>();

        originalCapsuleCenter = capsule.center;
        originalCapsuleHeight = capsule.height;
    }

    // Update is called once per frame
    void Update()
    {
        MovementAndRotation();
        Jumping();
        CrouchCheck();
        //UpdateAnimator();
    }

    private void FixedUpdate() {
        if (move.sqrMagnitude > 0.01f) {
            var targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));

            // FIXME : Doesn't stop smoothly due to this variable
            Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + currentSpeed * Time.fixedDeltaTime * direction.normalized);
        }

        if (jump) {
            rb.AddForce(new Vector3(0f, targetJumpHeight, 0f), ForceMode.Impulse);
            jump = false;
        }
    }

    void MovementAndRotation() {
        move = inputHandler.GetMoveInput();
        move = cameraTransform.TransformVector(move);

        IsRunning = inputHandler.GetRunInput() && move.sqrMagnitude > 0.01f && !IsInCrouchZone();

        float targetSpeed = IsRunning ? runSpeed : IsCrouched ? crouchWalkSpeed : walkSpeed;

        if (IsRunning) IsCrouched = false;
        
        if (move.sqrMagnitude < 0.01f) targetSpeed = 0;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, speedChangeSmoothTime);
        currentSpeed = Mathf.Round(currentSpeed * 1000f) / 1000f; // Round to 3 decimal places
    }

    void Jumping() {
        if (IsGrounded()) {
            if (jumpDelayTimer >= 0f)
                jumpDelayTimer -= Time.deltaTime;

            if (inputHandler.GetJumpInput() && jumpDelayTimer <= 0f) {
                if (IsInCrouchZone()) return;

                targetJumpHeight = IsCrouched ? crouchJumpHeight : jumpHeight;

                jump = true;
                IsCrouched = false;
            }
        } else {
            jumpDelayTimer = jumpDelayAfterLanding;
        }
    }

    private void CrouchCheck() {
        if (IsGrounded()) {
            if (inputHandler.GetCrouchInput() && !IsInCrouchZone()) {
                IsCrouched = !IsCrouched;
            }
        }

        float targetHeight = IsCrouched ? originalCapsuleHeight / 2 : originalCapsuleHeight;
        Vector3 targetCenter = IsCrouched ? originalCapsuleCenter / 2 : originalCapsuleCenter;

        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * crouchSpeedRate);
        capsule.center = Vector3.Lerp(capsule.center, targetCenter, Time.deltaTime * crouchSpeedRate);
    }

    public bool IsGrounded() {
        Vector3 spherePosition = new(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private bool IsInCrouchZone() {
        Vector3 spherePosition = new(transform.position.x, transform.position.y + crouchZoneOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, crouchRadius, crouchLayer, QueryTriggerInteraction.Ignore) && IsCrouched;
    }

    // TODO : For third person animations
    //void UpdateAnimator() {
    //    //anim.SetBool("isWalking", move.sqrMagnitude > 0.01f);
    //    //anim.SetBool("isRunning", IsRunning);
    //    //anim.SetBool("isCrouched", IsCrouched);
    //    //anim.SetBool("isGrounded", IsGrounded()) ;

    //    float targetAnimationMove = IsRunning ? 1f : 0.5f;
    //    if (move.sqrMagnitude < 0.01f) targetAnimationMove = 0f;

    //    animatorMovementSpeed = Mathf.Lerp(animatorMovementSpeed, targetAnimationMove, Time.deltaTime * 5f);
    //    anim.SetFloat("move", animatorMovementSpeed, 0.1f, Time.deltaTime);
    //}

    private void OnDrawGizmosSelected() {
        Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = IsGrounded() ? transparentGreen : transparentRed;

        Vector3 groundPosition = new(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        Gizmos.DrawSphere(groundPosition, groundRadius);

        Gizmos.color = IsInCrouchZone() ? transparentRed : transparentGreen;

        Vector3 crouchPosition = new(transform.position.x, transform.position.y + crouchZoneOffset, transform.position.z);
        Gizmos.DrawSphere(crouchPosition, crouchRadius);
    }
}
