using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrashCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private TrashStash stash;
    private int pickUpCount;

    private void Awake()
    {
        TrashStash.OnInteract += UpdateCounter;
    }

    private void OnDestroy()
    {
        TrashStash.OnInteract -= UpdateCounter;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCounter(0);
    }

    void UpdateCounter(int amount) {
        text.text = $"Trash in the stash: {amount}";
    }
}
