using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private float fade = 1.5f;
    [SerializeField] private float delayBeforeStarting = 2f;

    private float fadeTimer;
    private bool gameIsStarting;

    private void Start()
    {
        canvas.gameObject.SetActive(false);
        creditsPanel.SetActive(false);
        canvas.alpha = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsStarting) {
            float timeRatio = (Time.time - fadeTimer) / fade;
            canvas.alpha = timeRatio;

            if (timeRatio >= delayBeforeStarting) {
                SceneManager.LoadScene(1);
                gameIsStarting = false;
            }
        }

        if (creditsPanel.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame) {
            OpenCredits(false);
        }

    }

    public void StartGame() {
        gameIsStarting = true;
        fadeTimer = Time.time;
        canvas.gameObject.SetActive(true);
    }

    public void OpenCredits(bool value) {
        creditsPanel.SetActive(value ? true : false);
    }
}
