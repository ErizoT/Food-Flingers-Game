using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 2;

    private void Update()
    {
        if (playerHealth == 0)
        {
            Debug.Log(this.gameObject + " has died!");
        }
    }

    public void OnHit()
    {
        playerHealth -= 1;

        //Play some hurt effect or something
    }
}
