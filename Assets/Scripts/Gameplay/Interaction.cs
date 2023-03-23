using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactionDistance = 2.5f;

    private GameFlowManager manager;
    private TrashStash stash;
    private Transform cameraTransform;

    public static event Action<string> OnLook;
    public static event Action OnAway;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameFlowManager>();
        stash = GetComponent<TrashStash>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.gameState != GameFlowManager.GameState.InProgress) return;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactionDistance, interactableLayer)) {

            if (hit.collider.TryGetComponent<IInteractable>(out var i)) {

                OnLook?.Invoke(i.Message());

                if (Mouse.current.leftButton.wasPressedThisFrame) {
                    i.Interact();
                }
            }
        } else {
            OnAway?.Invoke();
        }
    }
}
