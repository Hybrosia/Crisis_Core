using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class WeaponMain : MonoBehaviour
{
    [SerializeField] public WeaponStats weaponStats;
    [SerializeField] private GameObject currentWeaponItem;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bullet; 

    public List<WeaponStats> weaponStatsList = new List<WeaponStats>();
    [SerializeField] private List<GameObject> weaponItem;
    [SerializeField] private List<GameObject> bullets;

    private BreathManager _breathManager;

    private float _availableGuns; 
    public int CurrentWeapon { get; private set; }
    private int _currentBullet; 
    
    public bool IsSwap { get; private set; }
    public float SwapTimer { get; private set; }
    public float TimeToSwap { get; private set; }

    private float _timeSinceLastShot;

    private Vector3 _shootDirection;
    private Camera _camera; 
    void Start()
    {
        _camera = Camera.main;
        _breathManager = GetComponent<BreathManager>();

        CurrentWeapon = 0; 
        
        foreach (var weapon in weaponItem)
        {weapon.SetActive(false);}
        
        weaponItem[CurrentWeapon].SetActive(true);
        weaponStats = weaponStatsList[CurrentWeapon];
    }

    
    void Update()
    {
        weaponStats = weaponStatsList[CurrentWeapon];
        currentWeaponItem = weaponItem[CurrentWeapon];
        bullet = bullets[CurrentWeapon];

        _timeSinceLastShot += Time.deltaTime;
        if (Time.deltaTime > SwapTimer + TimeToSwap)
        {
            IsSwap = false; 
        }
        
        OnCycleWeapon();
        
        if (InputManager.FirePressed)
        {
            OnFire();
        }
    }

    private bool CanShoot() => _timeSinceLastShot > 1f / (weaponStats.fireRate / 60) &&
                               _breathManager.Breath > weaponStats.breathUsage && !IsSwap;

    private void OnCycleWeapon()
    {
        if (InputManager.NextPressed)
        {
            print("cyclePressed");
            if (CurrentWeapon < weaponStatsList.Count - 1)
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon++; 
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time; 
            }
            else
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon = 0;
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time; 
            }
        }

        if (InputManager.PreviousPressed)
        {
            if (CurrentWeapon < 1)
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon = weaponStatsList.Count - 1;
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time; 
            }
            else
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon--; 
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time; 
            }
        }
            
    }

    private void OnFire()
    {
        if (!CanShoot()) return;

        var screenCentreCoordinates = new Vector3(0.5f, 0f, 0f);
        var ray = _camera.ViewportPointToRay(screenCentreCoordinates);
        // if it doesn't hit anything, make our projectile target 1000 away from us (adjust this accordingly)
        _shootDirection = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(1000f);

        GameObject bulletInstance = Instantiate(bullet, spawnPoint.transform.position, quaternion.identity);
        bulletInstance.GetComponent<Rigidbody>().linearVelocity = spawnPoint.transform.forward * weaponStats.bulletSpeed;
        Destroy(bulletInstance, 6f);

        _timeSinceLastShot = 0f;
        _breathManager.Breath -= weaponStats.breathUsage;
        _breathManager.timeSinceLastBreathUse = Time.time;
    }
}
