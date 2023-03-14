using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStash : MonoBehaviour
{
    [SerializeField] private int maxAmountCanHold = 5;

    public int stash;

    public static event Action<int> OnThrowAway;

    public void PickUp(GameObject trash) {
        if (stash >= maxAmountCanHold) {
            DisplayMessageEvent evt = Events.DisplayMessageEvent;
            evt.Message = "Your collection is full! Find a trash bin and throw it out.";
            EventManager.Broadcast(evt);
            return;
        }

        stash++;

        EventManager.Broadcast(Events.PickupEvent);

        Destroy(trash);
    }

    public void ThrowAway() {
        if (stash == 0) {
            DisplayMessageEvent evt = Events.DisplayMessageEvent;
            evt.Message = "You don't have any trash to throw out. Get back to work!";
            EventManager.Broadcast(evt);
            return;
        }

        OnThrowAway?.Invoke(stash);

        stash = 0;
        // This will update the trash text counter
        EventManager.Broadcast(Events.PickupEvent);
    }
}
