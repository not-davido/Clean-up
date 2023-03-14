using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        Interaction.OnLook += Message;
        Interaction.OnAway += Hide;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Interaction.OnLook -= Message;
        Interaction.OnAway -= Hide;
    }

    void Message(string message) {
        gameObject.SetActive(true);
        text.text = message;
    }

    void Hide() {
        gameObject.SetActive(false);
    }
}
