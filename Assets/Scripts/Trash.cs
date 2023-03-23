using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private void Awake()
    {
        var trashManager = FindObjectOfType<TrashManager>();
        trashManager.Add();
    }

    public void Interact()
    {
        TrashStash stash = FindObjectOfType<TrashStash>();
        stash.PickUp(gameObject);
    }

    public string Message() {
        return "Left click to pick up.";
    }
}
