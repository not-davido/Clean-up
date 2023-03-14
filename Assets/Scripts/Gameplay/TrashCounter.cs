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
        //TrashStash.OnPickup += UpdateCounter;

        EventManager.AddListener<PickupEvent>(UpdateCounter);
    }

    private void OnDestroy()
    {
        //TrashStash.OnPickup -= UpdateCounter;
    }

    // Start is called before the first frame update
    void Start()
    {
        stash = FindObjectOfType<TrashStash>();
        pickUpCount = stash.stash;
        text.text = $"Trash in the stash: {pickUpCount}";
    }

    void UpdateCounter(PickupEvent evt) {
        pickUpCount = stash.stash;
        text.text = $"Trash in the stash: {pickUpCount}";
    }
}
