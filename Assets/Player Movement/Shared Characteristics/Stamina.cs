using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float staminaChangeRate = 0.5f;
    [SerializeField] private float staminaDepletionCooldown = 4f;

    private FirstPersonController player;
    private PlayerInputHandler playerInputHandler;

    public float CurrentStamina { get; private set; }
    public bool CanRun { get; private set; }

    public float MaxStamina => maxStamina;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<FirstPersonController>();
        playerInputHandler = GetComponent<PlayerInputHandler>();

        CurrentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) {
            CanRun = true;
            RegenerateStamina();
            return;
        }

        if (player.IsRunning) {

            if (CurrentStamina > 0f) {
                CurrentStamina -= staminaChangeRate * Time.deltaTime;
            }

            if (CurrentStamina <= 0f) {
                CurrentStamina = -staminaDepletionCooldown;
                CanRun = false;
            }

        } else {
            RegenerateStamina();

            if (!CanRun) {
                // Check if the user is still holding the sprint key after stamina runs out
                bool stillHoldingRunKey = playerInputHandler.GetRunInput();

                if (CurrentStamina > 0f) {
                    if (!stillHoldingRunKey) {
                        CanRun = true;
                    }
                }
            }
        }
    }

    void RegenerateStamina() {
        CurrentStamina += (CurrentStamina < 0f ? 1f : staminaChangeRate) * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, -staminaDepletionCooldown, maxStamina);
    }
}
