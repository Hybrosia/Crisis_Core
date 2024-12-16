using UnityEngine;

public class ClearLevelTrigger : MonoBehaviour
{
    //Saves the player's progress and loads the next level.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        SaveScript.CurrentSave.LevelIndex = SceneData.CurrentSceneData.levelIndex + 1;
        SaveScript.CurrentSave.CheckpointIndex = 0;
        //TODO: SaveScript.CurrentSave.Spirit = Player.spirit; ? Er ikke helt sikker på om jeg skal lagre dette?
        
        SaveScript.WriteSaveToDisk();
        SaveScript.LoadFromDisk();
    }
}
