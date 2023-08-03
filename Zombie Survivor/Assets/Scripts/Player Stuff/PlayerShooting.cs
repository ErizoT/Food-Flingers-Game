using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject hitbox;
    //public bool isFiring = false;

    private void Update()
    {
        //hitbox.SetActive(false);
        //isFiring = false;
    }

    public void OnFire()
    {
        StartCoroutine(Firing());
    }

    IEnumerator Firing()
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.SetActive(false);
        StopCoroutine(Firing());
    }
}
