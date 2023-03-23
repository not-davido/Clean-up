using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;

public class GameFlowManager : MonoBehaviour {

    public enum GameState {
        Starting,
        Started,
        InProgress,
        Ending,
        Ended
    }

    public GameState gameState;

    [SerializeField] PlayableDirector cutscene;

    [Space(10)]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject gameEndingPanel;

    [Space(10)]
    [SerializeField] private float fadeOutTimer = 2;
    [SerializeField] private float delayBeforeFadingOut = 1;
    [SerializeField] private AudioClip gameFinishedAudio;

    [Space(10)]
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] TMPro.TextMeshProUGUI gameFinishedTime;
    [SerializeField] private List<string> messages = new();

    private float timer;
    private float endAudioTimer;
    private int currentMessageIndex;
    private bool gameIsEnding;

    public static event Action<bool> OnGameStart;

    private void Awake() {
        EventManager.AddListener<AllObjectivesCompletedEvent>(ObjectiveCompleted);

        cutscene.stopped += _ => StartGame();
    }

    // Start is called before the first frame update
    void Start() {
        fadeCanvas.alpha = 1;
        fadeCanvas.gameObject.SetActive(true);
        HUD.SetActive(false);
        gameEndingPanel.SetActive(false);
        text.text = string.Empty;
    }

    // Update is called once per frame
    void Update() {
        switch (gameState) {
            case GameState.Starting:
                StartingUpdate();
                break;
            case GameState.Started:
                StartedUpdate();
                break;
            case GameState.InProgress:
                InProgressUpdate();
                break;
            case GameState.Ending:
                EndingUpdate();
                break;
            case GameState.Ended:
                EndedUpdate();
                break;
        }
    }

    void ObjectiveCompleted(AllObjectivesCompletedEvent evt) => EndGame();

    void StartingUpdate() {
        if (currentMessageIndex < messages.Count) {
            ShowMessage(messages[currentMessageIndex]);
        } else {
            text.text = string.Empty;
            timer = Time.time + delayBeforeFadingOut;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(PlayCutscene());

            gameState = GameState.Started;
        }
    }

    void StartedUpdate() {
        float t = 1 - (Time.time - timer) / fadeOutTimer;
        fadeCanvas.alpha = t;

        if (t <= 0) {
            fadeCanvas.gameObject.SetActive(false);
        }
    }

    void InProgressUpdate() {
        if (gameIsEnding) {
            gameState = GameState.Ending;
        }
    }

    void EndingUpdate() {
        if (Time.time > endAudioTimer && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) {
            timer = Time.time;
            fadeCanvas.gameObject.SetActive(true);
            gameState = GameState.Ended;
        }
    }

    void EndedUpdate() {
        float t = (Time.time - timer) / fadeOutTimer;
        fadeCanvas.alpha = t;

        if (t >= 1 && t < 1.5f) {
            System.TimeSpan ts = System.TimeSpan.FromSeconds(GetComponentInChildren<GameTimer>().timer);
            var timeText = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            gameFinishedTime.text = $"Game time {timeText}";
        }

        if (t >= 2) {
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                SceneManager.LoadScene(0);
        }
    }

    void ShowMessage(string message) {
        if (text.text == string.Empty)
            text.text = message;

        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) {
            currentMessageIndex++;
            text.text = string.Empty;
        }
    }

    void StartGame() {
        HUD.SetActive(true);
        gameState = GameState.InProgress;

        OnGameStart?.Invoke(true);
        EventManager.Broadcast(Events.GameStartEvent);
    }

    void EndGame() {
        gameIsEnding = true;

        gameEndingPanel.SetActive(true);
        var text = gameEndingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = "Congrats, man! You cleaned up the park. Now get out.";

        AudioSource.PlayClipAtPoint(gameFinishedAudio, FindObjectOfType<FirstPersonController>().transform.position);
        endAudioTimer = gameFinishedAudio.length + Time.time;

        OnGameStart?.Invoke(false);
    }

    IEnumerator PlayCutscene() {
        yield return new WaitForSeconds(1f);
        cutscene.Play();
    }

    private void OnDisable() {
        EventManager.RemoveListener<AllObjectivesCompletedEvent>(ObjectiveCompleted);
    }
}