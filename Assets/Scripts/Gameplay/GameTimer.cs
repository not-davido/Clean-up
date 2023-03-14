using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private bool timerStarted;
    public bool gameEnded { get; set; }
    public float timer { get; private set; }

    private void Awake()
    {
        EventManager.AddListener<GameStartEvent>(StartTimer);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded) return;

        if (timerStarted) {
            timer += Time.deltaTime;
            System.TimeSpan ts = System.TimeSpan.FromSeconds(timer);
            //text.text = $"{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            text.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }
    }

    void StartTimer(GameStartEvent evt) {
        timerStarted = true;
    }

    private void OnDestroy() {
        EventManager.RemoveListener<GameStartEvent>(StartTimer);
    }
}
