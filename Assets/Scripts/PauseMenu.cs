using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuRoot;
    public TMPro.TextMeshProUGUI sliderValue;

    public bool canPause;

    private void Awake()
    {
        PlayerInputHandler.OnSensitivityChanged += SliderValue;
    }

    private void OnDestroy()
    {
        PlayerInputHandler.OnSensitivityChanged -= SliderValue;
    }

    // Start is called before the first frame update
    void Start() {
        menuRoot.SetActive(false);

        EventManager.AddListener<GameStartEvent>(CanPause);
        EventManager.AddListener<GameCompletedEvent>(CantPause);

        sliderValue.text = FindAnyObjectByType<PlayerInputHandler>().Sensitivity.ToString();
    }

    // Update is called once per frame
    void Update() {
        if (!canPause) return;

        if (!menuRoot.activeSelf && Mouse.current.rightButton.wasPressedThisFrame) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //if (Keyboard.current.escapeKey.wasPressedThisFrame) {
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}

        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            SetPauseMenuActivation(!menuRoot.activeSelf);
        }
    }

    void SetPauseMenuActivation(bool active) {
        menuRoot.SetActive(active);

        if (menuRoot.activeSelf) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    public void ClosePauseMenu() {
        SetPauseMenuActivation(false);
    }

    public void Quit() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    void CanPause(GameStartEvent evt) {
        canPause = true;
    }

    void CantPause(GameCompletedEvent evt) {
        canPause = false;
    }

    void SliderValue(float value) {
        sliderValue.text = value.ToString();
    }
}
