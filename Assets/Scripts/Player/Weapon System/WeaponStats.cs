using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Scriptable Objects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float fireRate;
    public float bulletSpeed;
    public float breathUsage;
    public float weaponDamage;
    public float swapTime; 
}
