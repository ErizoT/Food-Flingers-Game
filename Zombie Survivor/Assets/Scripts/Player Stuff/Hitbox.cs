using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public GameObject enemyHit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {

            Destroy(other.gameObject);
        }
    }
}
