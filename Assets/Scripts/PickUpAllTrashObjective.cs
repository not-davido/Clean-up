using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAllTrashObjective : Objective
{
    private int remainingTrash;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        EventManager.AddListener<TrashDumpedEvent>(OnThrowOut);
    }

    void OnThrowOut(TrashDumpedEvent evt) {
        if (IsCompleted) return;

        int remaining = evt.RemainingTrashCount;

        if (remaining == 0) {
            CompleteObjective();
        }
    }
}
