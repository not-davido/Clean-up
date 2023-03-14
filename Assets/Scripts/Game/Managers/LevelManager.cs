using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public enum State {
        Starting,
        Ending,
        Fade
    }

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private float fadeInLevel;
    [SerializeField] private float fadeOutLevel;

    private State levelState;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        levelState = State.Fade;
    }

    // Update is called once per frame
    void Update()
    {
        switch (levelState) {
            case State.Starting:
                FadeIn(true);
                break;
            case State.Ending:
                FadeIn(false);
                break;
        }
    }

    void FadeIn(bool value) {
        if (value) {
            float t = (Time.time - timer) / fadeInLevel;
            canvas.alpha = t;
        } else {
            float t = 1 - (Time.time - timer) / fadeOutLevel;
            canvas.alpha = t;
        }
    }
}
