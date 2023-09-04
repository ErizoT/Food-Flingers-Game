using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float projectileVelocity;
    public bool isThrown;
    public Rigidbody rb;
    public GameObject userThrowing;
    public GameObject spawnZone;

    [SerializeField] Material neutralMaterial;
    [SerializeField] Material selectedMaterial;

    enum projectileBehaviour
    {
        Straightforward,
        Bouncing,
        Homing,
        Splash
    }

    [SerializeField] projectileBehaviour projectileType;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        userThrowing = this.gameObject;
    }

    public void Update()
    {
        if (userThrowing.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (controller.selectedProjectile == this.gameObject)
            {
                GetComponent<MeshRenderer>().material = selectedMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = neutralMaterial;
                userThrowing = this.gameObject;
            }
        }
        
    }

    private void FixedUpdate ()
    {
        if (isThrown)
        {
            switch (projectileType)
            {
                case projectileBehaviour.Straightforward:
                    rb.MovePosition(transform.position + transform.forward * projectileVelocity * Time.deltaTime);
                    break;

                // Add more switch cases here once more projectile behaviours are developed!

                default:
                    return;
            }
        }
    }

    public void OnCollisionEnter(Collision col)
    {
        if (isThrown)
        {
            Debug.Log(this + "hit a wall");
            Destroy(this.gameObject);
            spawnZone.GetComponent<FoodSpawner>().spawnedProjectiles.Remove(this.gameObject);
        }

        if (isThrown && col.gameObject.tag == "Player")
        {
            spawnZone.GetComponent<FoodSpawner>().spawnedProjectiles.Remove(this.gameObject);

            Debug.Log(this + "hit" + col);
            col.gameObject.GetComponent<PlayerHealth>().OnHit();
            Destroy(this.gameObject);
        }
        
    }

    void HitVFX()
    {
        // This function will be called when the projectile hits anything
        // Set up here so that VFX and particles will fall out
    }
}
