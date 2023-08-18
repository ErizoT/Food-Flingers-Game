using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float projectileVelocity;
    public bool isThrown;
    public Rigidbody rb;

    // The length of time in which the projectile
    [SerializeField] float projectileTime;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate ()
    {
        if (isThrown)
        {
            rb.MovePosition(transform.position + transform.forward * projectileVelocity * Time.deltaTime);
            //transform.position += transform.forward * projectileVelocity * Time.deltaTime;
        }
    }

    public void OnCollisionEnter(Collision col)
    {
        if (isThrown)
        {
            Debug.Log(this + "hit a wall");
            gameObject.SetActive(false);
        }

        if (isThrown && col.gameObject.tag == "Player")
        {
            Debug.Log(this + "hit" + col);
            col.gameObject.GetComponent<PlayerHealth>().OnHit();
            gameObject.SetActive(false);
        }
        
    }

    void HitVFX()
    {
        // This function will be called when the projectile hits anything
        // Set up here so that VFX and particles will fall out
    }
}
