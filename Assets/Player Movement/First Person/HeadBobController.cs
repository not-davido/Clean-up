using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;

    [Header("Move")]
    [SerializeField] private float walkFrequency = 5f;
    [SerializeField] private float walkAmplitude = 0.02f;
    [SerializeField] private float runFrequency = 10f;
    [SerializeField] private float runAmplitude = 0.05f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Jump")]
    [SerializeField] private float jumpBobDuration = 0.1f;
    [SerializeField] private float jumpBobAmount = 0.25f;

    private FirstPersonController player;
    private Vector3 originalCameraHolderPosition;
    private Vector3 velocity;
    private float timer = 1f;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<FirstPersonController>();
        originalCameraHolderPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;

        Vector3 jumpOffset = new(0f, offset, 0f);

        if (player.HorizontalSpeed > 0.01f && player.IsGrounded()) {
            timer += Time.deltaTime;

            Vector3 wave = originalCameraHolderPosition;

            float targetFrequency = player.IsRunning ? runFrequency : walkFrequency;
            float targetAmplitude = player.IsRunning ? runAmplitude : walkAmplitude;

            wave.x += Mathf.Cos(timer * targetFrequency) * targetAmplitude;
            wave.y += Mathf.Sin(timer * targetFrequency * 2f) * targetAmplitude;

            MoveCamera(wave - jumpOffset);

        } else {
            MoveCamera(originalCameraHolderPosition - jumpOffset);
            timer = 1f;
        }
    }

    void MoveCamera(Vector3 position) {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, position, ref velocity, smoothTime);
    }

    public IEnumerator JumpBob() {
        // make the camera move down slightly
        float t = 0f;
        while (t < jumpBobDuration) {
            offset = Mathf.Lerp(0f, jumpBobAmount, t / jumpBobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // make it move back to neutral
        t = 0f;
        while (t < jumpBobDuration) {
            offset = Mathf.Lerp(jumpBobAmount, 0f, t / jumpBobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        offset = 0f;
    }
}
