using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour {

    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    private bool startTimer;
    private float timeElapsed;

    public float timer { get; private set; }

    private void Awake()
    {
        GameFlowManager.OnGameStart += StartTimer;
    }

    float nextTime;

    // Update is called once per frame
    void Update() {
        if (!startTimer) return;

        timer += Time.deltaTime;
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 1) {
            System.TimeSpan ts = System.TimeSpan.FromSeconds(timer);
            //text.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            text.text = $"{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            timeElapsed %= 1;
        }
    }

    public void StartTimer(bool value) {
        startTimer = value;
    }

    private void OnDisable() {
        GameFlowManager.OnGameStart -= StartTimer;
    }
}

