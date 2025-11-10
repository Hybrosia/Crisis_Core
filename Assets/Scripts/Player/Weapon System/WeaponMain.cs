using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class WeaponMain : MonoBehaviour
{
    [SerializeField] public WeaponStats weaponStats;
    [SerializeField] private GameObject currentWeaponItem;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Animator weaponSelectAnimator, weaponAnimator;

    public List<WeaponStats> weaponStatsList = new List<WeaponStats>();
    [SerializeField] private List<GameObject> weaponItem;
    [SerializeField] private List<GameObject> bullets;

    private AltFireScript _altFireScript;

    private BreathManager _breathManager;

    private float _availableGuns; 
    public int CurrentWeapon { get; private set; }
    private int _currentBullet; 
    
    public bool IsSwap { get; private set; }
    public float SwapTimer { get; private set; }
    public float TimeToSwap { get; private set; }

    public float _timeSinceLastShot;

    private SpellsAndAbilities _spells;
    
    private Camera _camera; 
    void Start()
    {
        _camera = Camera.main;
        _breathManager = GetComponent<BreathManager>();

        CurrentWeapon = 0;

        _spells = GetComponent<SpellsAndAbilities>();
        
        foreach (var weapon in weaponItem)
        {weapon.SetActive(false);}
        
        weaponItem[CurrentWeapon].SetActive(true);
        weaponStats = weaponStatsList[CurrentWeapon];

        _altFireScript = GetComponent<AltFireScript>();
    }

    
    void Update()
    {
        weaponStats = weaponStatsList[CurrentWeapon];
        currentWeaponItem = weaponItem[CurrentWeapon];
        bullet = bullets[CurrentWeapon];

        TimeToSwap = weaponStats.swapTime;
        
        _timeSinceLastShot += Time.deltaTime;
        if (Time.time > SwapTimer + TimeToSwap)
        {
            IsSwap = false; 
        }
        
        if (!IsSwap) OnCycleWeapon();
        
        if ((!weaponStats.isAutomatic && InputManager.FirePressed) || (weaponStats.isAutomatic && InputManager.FireHeld))
        {
            OnFire();
        }
        else
        {
            weaponAnimator.SetBool("Fire", false);
        }

        if (InputManager.AltFirePressed)
        {
            OnAltFire();
        }

        if (InputManager.AltFireReleased)
        {
            OnAltRelease();
        }
    }

    public bool CanShoot() => _timeSinceLastShot > weaponStats.fireRate &&
               _breathManager.Breath >= weaponStats.breathUsage && !IsSwap && !_spells.isSpell;

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
                if (CurrentWeapon == 1)
                {
                    weaponSelectAnimator.Play("RightBlower");
                    weaponAnimator.Play("Blower");
                }
                else if (CurrentWeapon == 2)
                {
                    weaponSelectAnimator.Play("RightSniper");
                    weaponAnimator.Play("Sniper");
                }
            }
            else
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon = 0;
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time;
                weaponSelectAnimator.Play("RightGun");
                weaponAnimator.Play("BubbleGun");
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
                weaponSelectAnimator.Play("LeftSniper");
                weaponAnimator.Play("Sniper");
            }
            else
            {
                currentWeaponItem.SetActive(false);
                CurrentWeapon--; 
                currentWeaponItem.SetActive(true);
                IsSwap = true;
                SwapTimer = Time.time;
                if (CurrentWeapon == 0)
                {
                    weaponSelectAnimator.Play("LeftGun");
                    weaponAnimator.Play("BubbleGun");
                }
                else if (CurrentWeapon == 1)
                {
                    weaponSelectAnimator.Play("LeftBlower");
                    weaponAnimator.Play("Blower");
                }
            }
        }
    }

    private void OnFire()
    {
        if (!CanShoot()) return;
        weaponAnimator.SetBool("Fire", true);

        var screenCentreCoordinates = new Vector3(0.5f, 0f, 0f);
        var ray = _camera.ViewportPointToRay(screenCentreCoordinates);
        // if it doesn't hit anything, make our projectile target 1000 away from us (adjust this accordingly)

        GameObject bulletInstance = Instantiate(bullet, spawnPoint.transform.position, quaternion.identity);
        bulletInstance.GetComponent<Rigidbody>().linearVelocity = spawnPoint.transform.forward * weaponStats.bulletSpeed;
        Destroy(bulletInstance, 6f);

        _timeSinceLastShot = 0f;
        _breathManager.UseBreath(weaponStats.breathUsage);
    }

    private void OnAltFire()
    {
        if (!CanShoot()) return;
        _altFireScript.castAltInit(CurrentWeapon);
        print("Initiated fire");
    }

    private void OnAltRelease()
    {
        _altFireScript.castAltFinish(CurrentWeapon);
        print("exited fire");
    }
    
}
