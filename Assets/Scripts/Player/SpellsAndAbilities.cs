using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellsAndAbilities : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpellBullets;
    private float DashCooldown;
    private float WaveCooldown;
    private float SpringMistCooldown;

    private float DashUsed;
    private float WaveUsed;
    private float MistUsed;

    private float DashDuration;

    private bool isDash;
    private bool isWave;
    private bool isMist;

    [SerializeField] private float dashSpeed; 

    private float waveSpeed;
    private float waveLifetime;

    private float SpringMistSpeed;
    private float SpringMistLifetime; 

    [SerializeField] private GameObject _spawnPoint; 

    public bool isSpell { get; private set; }

    private void Start()
    {
        isSpell = false;
        DashUsed = Time.time - DashCooldown;
        WaveUsed = Time.time - WaveCooldown;
        MistUsed = Time.time - MistUsed; 
    }

    // Update is called once per frame
    void Update()
    {
        if (isDash || isWave || isMist)
        {
            isSpell = true; 
        }
    }

    private void Dash()
    {
        if (Time.time > DashUsed + DashCooldown)
        {
            DashUsed = Time.time;
            isDash = true; 
            
            /*GameObject bulletInstance = Instantiate(SpellBullets[0]
             
             Instantiate on player position with 0 velocity
             
             Add negative velocity to Movement.cs when activated if standing still, else add velocity in direction you're moving in*/
        }
    }

    private void Wave()
    {
        if (Time.time > WaveUsed + DashCooldown)
        {
            WaveUsed = Time.time;
            isWave = true; 
            
            /*GameObject bulletInstance = Instantiate(SpellBullets[1], _spawnPoint.transform.position, _spawnPoint.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = _spawnPoint.transform.forward * waveSpeed;
            Destroy(bulletInstance, waveLifetime);
            
            GameObject bulletInstance1 = Instantiate(SpellBullets[1], _spawnPoint.transform.position, _spawnPoint.transform.rotation);
            bulletInstance1.GetComponent<Rigidbody>().linearVelocity = (Quaternion.Euler(0,30,0) * _spawnPoint.transform.forward) * waveSpeed;
            Destroy(bulletInstance1, waveLifetime);
            
            GameObject bulletInstance2 = Instantiate(SpellBullets[1], _spawnPoint.transform.position, _spawnPoint.transform.rotation);
            bulletInstance2.GetComponent<Rigidbody>().linearVelocity = (Quaternion.Euler(0,-30,0) * transform.forward) * waveSpeed;
            Destroy(bulletInstance2, waveLifetime);*/
            //Move spawning into animation, set isWave false when animation ends 
        }
    }

    private void SpingWave()
    {
        if (Time.time > MistUsed + SpringMistCooldown)
        {
            GameObject bulletInstance = Instantiate(SpellBullets[1], _spawnPoint.transform.position, _spawnPoint.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = _spawnPoint.transform.forward * SpringMistSpeed;
            Destroy(bulletInstance, SpringMistLifetime);
        }
    }
}
