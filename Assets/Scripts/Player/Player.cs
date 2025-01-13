using System;
using UnityEngine;

public class Player : MonoBehaviour
{
   public PlayerData PlayerData;

   private void Start()
   {
      PlayerData.player = transform;
   }

   private void Update()
   {
      PlayerData.PlayerPos = transform.position;
   }
}
