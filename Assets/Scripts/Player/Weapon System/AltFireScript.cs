using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AltFireScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> AltBullets;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float RushFireMinSpeed, RushFireMaxSpeed;
    [SerializeField] private float RushFireMinLifetime, RushFireMaxLifetime;
    
    [SerializeField] private float BounceFireMaxSpeed;
    [SerializeField] private float BounceFireMinLifetime, BounceFireMaxLifetime;

    
    [SerializeField] private float rushFireMin, rushFireMax;
    [SerializeField] private float bounceFireMax;
    [SerializeField] private float trapFireMin; 

    private float TimerInit;
    private float TimerExit;
    
    
    
    
    
    public void castAltInit()
    {
        TimerInit = Time.time;
        print("Started firing timer");
    }

    public void castAltFinish(int weaponNumber)
    {
        switch (weaponNumber)
        {
            case 0: altFire0();
                break;
            case 1 : altFire1();
                break;
            case 2: altFire2();
                break;
        }
    }

    private void altFire0()
    {
        TimerExit = Time.time; 
        
        if (TimerExit - TimerInit >= rushFireMax)
        {
            GameObject bulletInstance = Instantiate(AltBullets[0], spawnPoint.transform.position, spawnPoint.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = spawnPoint.transform.forward * RushFireMaxSpeed;
            Destroy(bulletInstance, RushFireMaxLifetime);
        }
        else if (TimerExit - TimerInit >= rushFireMin)
        {
            GameObject bulletInstance = Instantiate(AltBullets[0], spawnPoint.transform.position, spawnPoint.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = spawnPoint.transform.forward * RushFireMinSpeed;
            Destroy(bulletInstance, RushFireMinLifetime);
            
        }
    }

    private void altFire1()
    {
        TimerExit = Time.time; 
        
        if (TimerExit - TimerInit >= rushFireMax)
        {
            GameObject bulletInstance = Instantiate(AltBullets[1], spawnPoint.transform.position, spawnPoint.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = spawnPoint.transform.forward * BounceFireMaxSpeed;
            Destroy(bulletInstance, BounceFireMaxLifetime);
        }
    }
    
    private void altFire2()
    {
        throw new NotImplementedException();
    }
}
