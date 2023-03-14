using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;
    [SerializeField] private float webglLookSensitivityMultiplier = 0.25f;
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    [SerializeField] private float inputSmoothTime = 0.2f;

    private GameFlowManager gameFlowManager;
    private PlayerControls controls;
    private Vector3 smoothInput;
    private Vector3 velocity;
    private bool canRun;

    public float Sensitivity => sensitivityX;
    public bool CanUseInput => Cursor.lockState == CursorLockMode.Locked && gameFlowManager.GameInProgress;

    private void Awake() {
        controls = new PlayerControls();

        controls.Gameplay.Run.performed += _ => {
            canRun = true;
        };

        controls.Gameplay.Run.canceled += _ => {
            canRun = false;
        };

        gameFlowManager = FindObjectOfType<GameFlowManager>();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public Vector3 GetMoveInput() {
        if (!CanUseInput) return Vector3.zero;

        Vector2 input = controls.Gameplay.Move.ReadValue<Vector2>();
        Vector3 move = new(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);
        smoothInput = Vector3.SmoothDamp(smoothInput, move, ref velocity, inputSmoothTime);
        return smoothInput;
    }

    public bool GetJumpInput() {
        if (!CanUseInput) return false;

        return controls.Gameplay.Jump.WasPressedThisFrame();
    }

    public float GetLookInputHorizontal() {
        if (!CanUseInput) return 0;

        var hLook = controls.Gameplay.Look.ReadValue<Vector2>().x * sensitivityX;

        if (invertX) hLook *= -1;

        hLook *= 0.1f;

#if UNITY_WEBGL
        hLook *= webglLookSensitivityMultiplier;
#endif

        return hLook;
    }

    public float GetLookInputVertical() {
        if (!CanUseInput) return 0;

        var vLook = controls.Gameplay.Look.ReadValue<Vector2>().y * sensitivityY * -1;

        if (invertY) vLook *= -1;

        vLook *= 0.1f;

#if UNITY_WEBGL
        vLook *= webglLookSensitivityMultiplier;
#endif

        return vLook;
    }

    public bool GetRunInput() {
        if (!CanUseInput) return false;

        return canRun;
    }

    public bool GetCrouchInput() {
        if (!CanUseInput) return false;

        return controls.Gameplay.Crouch.WasPressedThisFrame();
    }

    public static event System.Action<float> OnSensitivityChanged;

    public void UpdateSensitivity(float value) {
        sensitivityX = value;
        sensitivityY = value;

        OnSensitivityChanged?.Invoke(value);
    }
}
