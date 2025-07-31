using System.Collections.Generic;
using UnityEngine;

public class SniperPosition : MonoBehaviour
{
    [Tooltip("Includes all SniperPositions by default")] public List<SniperPosition> validJumps = new List<SniperPosition>();
    
    public static List<SniperPosition> ActiveSniperPositions = new List<SniperPosition>();
    
    private void OnEnable() => ActiveSniperPositions.Add(this);
    private void OnDisable() => ActiveSniperPositions.Remove(this);
}
