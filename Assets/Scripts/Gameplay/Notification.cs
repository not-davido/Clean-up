using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private GameObject notificationObject;

    [SerializeField] private float delayBeforeFadingOut = 3;
    [SerializeField] private float fadeOut = 1;

    private CanvasGroup canvas;
    private float timer;

    private void Awake()
    {
        var text = notificationObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = string.Empty;

        EventManager.AddListener<DisplayMessageEvent>(Notify);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<DisplayMessageEvent>(Notify);
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = notificationObject.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (notificationObject.activeInHierarchy) {
            if (canvas.alpha > 0) {
                canvas.alpha = 1 - (Time.time - timer) / fadeOut;
            } else {
                notificationObject.SetActive(false);
            }
        }
    }

    void Notify(DisplayMessageEvent evt) {
        notificationObject.SetActive(true);

        var text = notificationObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = evt.Message;

        timer = Time.time + delayBeforeFadingOut;

        canvas.alpha = 1;
    }
}
