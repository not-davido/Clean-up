using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public int trashRemaining { get; private set; }

    public void Add() {
        trashRemaining++;
    }

    public void Remove(int amount) {
        trashRemaining -= amount;

        TrashDumpedEvent evt = Events.TrashDumpedEvent;
        evt.RemainingTrashCount = trashRemaining;
        EventManager.Broadcast(evt);
    }
}
