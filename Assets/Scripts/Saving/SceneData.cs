using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData CurrentSceneData;
    
    public int levelIndex;
    public List<GoddessShrine> goddessShrines = new List<GoddessShrine>();
    public NavigationPoint[] navigationPoints = Array.Empty<NavigationPoint>();

    //Registers itself as the current scene data.
    private void Awake()
    {
        CurrentSceneData = this;
    }

    //Moves the player to the correct place.
    private void Start()
    {
        goddessShrines[SaveScript.CurrentSave.CheckpointIndex].MovePlayerHere();
    }
}
