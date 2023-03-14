using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(PlayerInputHandler))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private bool enableRun;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float speedChangeRate = 3f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float jumpDelayAfterLanding = 0.2f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private bool debugGroundCheck;

    private PlayerInputHandler inputHandler;
    private Rigidbody2D rb2D;
    private BoxCollider2D boxCollider2D;
    private float horizontalMovement;
    private float currentSpeed;
    private float jumpTimer;
    private bool jump;
    private bool isRunning;
    
    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
        FlipSpriteCheck();

        if (debugGroundCheck)
            DebugGroundCheckStatus();
    }

    private void FixedUpdate() {
        var position = rb2D.position;
        position.x += horizontalMovement * currentSpeed * Time.fixedDeltaTime;
        rb2D.position = position;

        if (jump) {
            rb2D.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
            jump = false;
        }
    }

    void FlipSpriteCheck() {
        if (horizontalMovement > 0.1f) {
            transform.rotation = Quaternion.identity;
        } else if (horizontalMovement < -0.1f) {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    bool IsGrounded() {
        return Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, groundOffset, groundLayer);
    }

    void Movement() {
        isRunning = inputHandler.GetRunInput();

        horizontalMovement = inputHandler.GetMoveInput().x;

        float targetSpeed;

        targetSpeed = enableRun && isRunning ? runSpeed : walkSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
    }

    void Jump() {
        if (IsGrounded()) {
            if (inputHandler.GetJumpInput() && jumpTimer <= 0f) {
                jumpTimer = jumpDelayAfterLanding;
                jump = true;
            }

            if (jumpTimer > 0f) {
                jumpTimer -= Time.deltaTime;
            }
        }
    }

    void DebugGroundCheckStatus() {
        Color rayColor = IsGrounded() ? Color.green : Color.red;

        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + groundOffset), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + groundOffset), rayColor);
    }
}
