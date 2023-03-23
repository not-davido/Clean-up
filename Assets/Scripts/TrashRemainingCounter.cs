using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrashRemainingCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start() {
        text.text = $"Trash left to throw out: {FindObjectOfType<TrashManager>().trashRemaining}";

        EventManager.AddListener<TrashDumpedEvent>(UpdateCounter);
    }

    void UpdateCounter(TrashDumpedEvent evt) {
        text.text = $"Trash left to throw out: {evt.RemainingTrashCount}";
    }
}
