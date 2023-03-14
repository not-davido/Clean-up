using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Footsteps")]
public class FootstepScriptableObject : ScriptableObject
{
    [field: SerializeField, Header("Surface")] public TerrainLayer TerrainLayer { get; private set; }
    [field: SerializeField] public Surface Surface { get; private set; }

    
    [field: SerializeField, Header("Audio")] public AudioClip[] Footsteps { get; private set; }
    [field: SerializeField] public AudioClip[] Jump { get; private set; }
    [field: SerializeField] public AudioClip[] Land { get; private set; }

    public string GetSurfaceTag() => $"Surface/{Enum.GetName(typeof(Surface), Surface)}";
}

[Serializable]
public enum Surface {
    // Must be updated every time a new tag is created
    None, Wood, Grass, Mud, Concrete
}
