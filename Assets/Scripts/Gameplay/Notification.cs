using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private float delayBeforeFadingOut = 3;
    [SerializeField] private float fadeOut = 1;

    [SerializeField] private GameObject notificationObject;

    private CanvasGroup alpha;
    private float timer;

    private void Awake()
    {
        notificationObject.SetActive(false);
        var text = notificationObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = string.Empty;

        EventManager.AddListener<DisplayMessageEvent>(Notify);
    }

    // Start is called before the first frame update
    void Start()
    {
        alpha = notificationObject.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (notificationObject.activeInHierarchy) {
            if (alpha.alpha > 0) {
                alpha.alpha = 1 - (Time.time - timer) / fadeOut;
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

        alpha.alpha = 1 - (Time.time - timer) / fadeOut;

    }
}
