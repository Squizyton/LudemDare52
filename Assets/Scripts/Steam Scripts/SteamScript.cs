using System;

using UnityEngine;
using Steamworks;


namespace Steam_Scripts
{
    public class SteamScript : MonoBehaviour
    {
        private void Awake()
        {
            //Check to see if SteamManager is initialized
            if (!SteamManager.Initialized) return;
            
            var steamName = SteamFriends.GetPersonaName();
            Debug.Log(steamName);
        }
    }
}
