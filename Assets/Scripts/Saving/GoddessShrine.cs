using UnityEngine;

public class GoddessShrine : MonoBehaviour
{
    [SerializeField] private float distanceFromShrineToSpawnPoint = 2f;

    //Resets the player's health and breath and saves all other relevant data.
    public void Interact()
    {
        //TODO: Reset the player's health and breath

        SaveProgress();
    }

    //Moves the player to a position in front of the shrine and makes them look away from the shrine.
    public void MovePlayerHere()
    {
        var playerTransform = FindFirstObjectByType<Player>().transform;

        var offset = transform.forward;
        offset.y = 0f;
        offset = offset.normalized * distanceFromShrineToSpawnPoint;

        playerTransform.position = transform.position + offset;
        playerTransform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }
    
    //Saves the player's progress.
    private void SaveProgress()
    {
        SaveScript.CurrentSave.LevelIndex = SceneData.CurrentSceneData.levelIndex;
        SaveScript.CurrentSave.CheckpointIndex = SceneData.CurrentSceneData.goddessShrines.IndexOf(this);
        //TODO: SaveScript.CurrentSave.Spirit = Player.spirit;
        
        SaveScript.WriteSaveToDisk();
    }
}
