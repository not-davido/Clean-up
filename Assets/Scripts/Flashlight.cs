using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    GameFlowManager manager;

    private void Start()
    {
        _light.SetActive(false);

        manager = FindObjectOfType<GameFlowManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (manager.gameState != GameFlowManager.GameState.InProgress) return;

        if (Keyboard.current.fKey.wasPressedThisFrame) {
            _light.SetActive(!_light.activeInHierarchy);
        }
    }
}
