using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        TrashStash stash = FindObjectOfType<TrashStash>();
        stash.ThrowAway();
    }

    public string Message()
    {
        return "Left click to throw out trash.";
    }
}
