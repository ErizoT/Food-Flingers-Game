using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPage : MonoBehaviour
{
    public RespawnManager rM;


    public void Confirm()
    {
        Debug.Log("Confirmed");
        // Initiated by player 1 pressing 
        // Start coroutine in Respawn Manager for gamecountdown
        // Set tutorial object in respawn manager to null
        // Set tutorialPage on player to null
        // Destroy this gameObject

        rM.StartCoroutine(rM.GameCountdown());
        rM.tutorialPage = null;
        Destroy(gameObject, 0.1f);
    }
}
