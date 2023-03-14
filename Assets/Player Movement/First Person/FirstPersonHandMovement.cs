using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonHandMovement : MonoBehaviour
{
    [Header("Hand Bobbing")]
    [SerializeField] private float frequency = 10f;
    [SerializeField] private float amplitude = 0.03f;
    [SerializeField] private float bobbingSmoothTime = 0.3f;

    [Header("Hand Sway")]

    [SerializeField] [Range(0f, 1f)]
    private float swaySpeedMultiplier = 0.03f;

    [Tooltip("How far the hand will sway.")]
    [SerializeField] private float maxSway = 0.04f;

    [SerializeField] [Range(0f, 1f)]
    private float airSwaySpeedMultiplier = 0.8f;

    [Tooltip("How far the hand will sway in the air.")]
    [SerializeField] private float maxAirSway = 0.08f;

    [SerializeField] private float swaySmoothTime = 0.3f;

    private FirstPersonController player;
    private PlayerInputHandler playerInput;
    private Vector3 originalPosition;
    private Vector3 bobbingVelocity;
    private Vector3 swayVelocity;
    private float timer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<FirstPersonController>();
        playerInput = GetComponentInParent<PlayerInputHandler>();

        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Bobbing();
        Sway();
    }

    void Bobbing() {
        if (player.HorizontalSpeed > 0.01f && player.IsGrounded()) {
            timer += Time.deltaTime;

            Vector3 pos = originalPosition;
            pos.x += Mathf.Cos(timer * frequency) * amplitude;
            pos.y += Mathf.Cos(timer * frequency * 2) * amplitude;

            SmoothDampTo(pos, ref bobbingVelocity, bobbingSmoothTime);
        } else {
            timer = 1f;
            SmoothDampTo(originalPosition, ref bobbingVelocity, bobbingSmoothTime);
        }
    }

    void Sway() {
        float horizontalLook = playerInput.GetLookInputHorizontal();
        float verticalLook = playerInput.GetLookInputVertical();
        float verticalVelocity = player.Controller.velocity.y;
        float verticalMaxSway = player.IsGrounded() ? maxSway : maxAirSway;

        float swayX = horizontalLook != 0 ? (-horizontalLook * swaySpeedMultiplier) : player.HorizontalSpeed > 0.01f ? (-playerInput.GetMoveInput().x * swaySpeedMultiplier) : 0f;
        float swayY = player.IsGrounded() ? (verticalLook * swaySpeedMultiplier) : (verticalVelocity * airSwaySpeedMultiplier);
        float swayZ = player.HorizontalSpeed > 0.01f ? (-playerInput.GetMoveInput().z * swaySpeedMultiplier) : 0f;

        swayX = Mathf.Clamp(swayX, -maxSway, maxSway);
        swayY = Mathf.Clamp(swayY, -verticalMaxSway, verticalMaxSway);
        swayZ = Mathf.Clamp(swayZ, -maxSway, maxSway);

        Vector3 swayPosition = new Vector3(swayX, swayY, swayZ) + originalPosition;
        SmoothDampTo(swayPosition, ref swayVelocity, swaySmoothTime);
    }

    void SmoothDampTo(Vector3 position, ref Vector3 velocity, float smoothTime) {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, position, ref velocity, smoothTime);
    }
}
