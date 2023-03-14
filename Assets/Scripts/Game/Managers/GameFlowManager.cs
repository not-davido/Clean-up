using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject trashParent;
    [SerializeField] private GameObject gameEndingPanel;
    [SerializeField] private float fadeOutTimer = 2;
    [SerializeField] private float delayBeforeFadingOut = 1;
    [SerializeField] private AudioClip gameFinished;

    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] TMPro.TextMeshProUGUI trashLeftText;
    [SerializeField] TMPro.TextMeshProUGUI gameFinishedTime;
    [SerializeField] private List<string> messages = new();

    private GameTimer gameTimer;
    public int trashCount;
    private float timer;
    private int currentMessageIndex;
    private bool messagesDone;
    private bool introDone;
    private bool fadeOut;
    private bool playEndingAudio;
    private float endAudioTimer;

    public bool gameIsEnding { get; private set; }
    public bool GameInProgress { get; private set; }

    private void Awake()
    {
        TrashStash.OnThrowAway += TrashLeft;

        cutscene.played += _ => hud.SetActive(false);
        cutscene.stopped += _ => {
            hud.SetActive(true);
            GameInProgress = true;
            EventManager.Broadcast(Events.GameStartEvent);
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas.alpha = 1;
        canvas.gameObject.SetActive(true);

        foreach (Transform t in trashParent.transform) {
            if (t.gameObject.activeInHierarchy) {
                trashCount++;
            }
        }

        trashLeftText.text = $"Trash amount to throw out: {trashCount}";

        gameEndingPanel.SetActive(false);

        gameTimer = GetComponentInChildren<GameTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!introDone) {
            if (currentMessageIndex < messages.Count) {
                ShowMessage(messages[currentMessageIndex]);
            } else {
                messagesDone = true;
                text.gameObject.SetActive(false);
                introDone = true;
            }
        }

        if (messagesDone) {
            timer = Time.time + delayBeforeFadingOut;
            fadeOut = true;
            messagesDone = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(PlayCutscene());
        }

        if (fadeOut) {
            float t = 1 - (Time.time - timer) / fadeOutTimer;
            canvas.alpha = t;

            if (t <= 0) {
                canvas.gameObject.SetActive(false);
                fadeOut = false;
            }
        }

        if (!gameIsEnding) {
            if (trashCount == 0) {
                gameTimer.gameEnded = true;
                EndGame();
                EventManager.Broadcast(Events.GameCompletedEvent);
            }
        } else {
            canvas.gameObject.SetActive(true);
            float t = (Time.time - timer) / fadeOutTimer;
            canvas.alpha = t;

            if (t >= 1) {
                System.TimeSpan ts = System.TimeSpan.FromSeconds(gameTimer.timer);
                var timeText = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                gameFinishedTime.text = $"Game time {timeText}";

                if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                    SceneManager.LoadScene(0);
            }
        }

    }

    void ShowMessage(string message) {
        text.text = message;
        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            currentMessageIndex++;
    }

    void TrashLeft(int amount) {
        trashCount -= amount;
        trashLeftText.text = $"Trash amount to throw out: {trashCount}";
    }

    void EndGame() {
        gameEndingPanel.SetActive(true);
        var text = gameEndingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = "Congrats, man! You cleaned up the park. Now get out.";
        GameInProgress = false;

        if (!playEndingAudio) {
            AudioSource.PlayClipAtPoint(gameFinished, FindObjectOfType<FirstPersonController>().transform.position);
            endAudioTimer = gameFinished.length + Time.time;
            playEndingAudio = true;
        }

        if (Time.time > endAudioTimer && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) {
            gameIsEnding = true;
            timer = Time.time;
        }
    }

    IEnumerator PlayCutscene() {
        yield return new WaitForSeconds(1f);
        cutscene.Play();
    }
}
