using UnityEngine;

[CreateAssetMenu(menuName = "Scrubs")]
public class PlayerData : ScriptableObject
{
    public Transform player;
    public Vector3 PlayerPos;

    //Returns true if the player is visible from the target position, i.e., the player can see the point.
    public bool CanSeePlayerFromPoint(Vector3 targetPosition)
    {
        Physics.Raycast(targetPosition, Vector3.Normalize(PlayerPos - targetPosition), out var hitInfoTest,
            Vector3.Distance(PlayerPos, targetPosition), LayerMask.GetMask("Player", "Terrain"));
        Debug.Log(hitInfoTest.transform);
        if (!Physics.Raycast(targetPosition, Vector3.Normalize(PlayerPos - targetPosition), out var hitInfo,
                Vector3.Distance(PlayerPos, targetPosition), LayerMask.GetMask("Player", "Terrain"))) return false;
        return hitInfo.transform.CompareTag("Player");
    }
}