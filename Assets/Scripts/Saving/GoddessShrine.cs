using UnityEngine;

public class GoddessShrine : MonoBehaviour
{
    [SerializeField] private float distanceFromShrineToSpawnPoint = 2f;

    //Resets the player's health and breath and saves all other relevant data.
    public void Interact()
    {
        var breathManager = FindFirstObjectByType<BreathManager>();
        var playerHealth = FindFirstObjectByType<PlayerHealthManager>();
        
        if (breathManager) breathManager.ResetBreath();
        if (playerHealth) playerHealth.ResetHealth();

        SaveProgress();
    }

    //Moves the player to a position in front of the shrine and makes them look away from the shrine.
    public void MovePlayerHere()
    {
        var player = FindFirstObjectByType<Player>();
        if (!player) return;

        var offset = transform.forward;
        offset.y = 0f;
        offset = offset.normalized * distanceFromShrineToSpawnPoint;

        player.transform.position = transform.position + offset;
        player.transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }
    
    //Saves the player's progress.
    private void SaveProgress()
    {
        SaveScript.CurrentSave.LevelIndex = SceneData.CurrentSceneData.levelIndex;
        SaveScript.CurrentSave.CheckpointIndex = SceneData.CurrentSceneData.goddessShrines.IndexOf(this);
        
        SaveScript.WriteSaveToDisk();
    }
}
