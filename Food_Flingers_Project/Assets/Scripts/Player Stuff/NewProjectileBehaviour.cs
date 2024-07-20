using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NewProjectileBehaviour : MonoBehaviour
{
    public enum projectileBehaviour
    {
        straightforward,
        rebound,
        homing,
        splash
    }

    // Unviersal Projectile Settings
    [HideInInspector] public projectileBehaviour projectileType; // The type of projectile the object is

    [Header("Universal Projectile Settings")]
    [SerializeField] float thrownSpeed; // The thrown speed of the projectile
    [SerializeField] ParticleSystem splatParticle;
    [SerializeField] AudioClip splatSound;
    public Animator animator;

    private AudioSource audioSource;
    private TrailRenderer trailRenderer;
    [Space(10)]
    
    // Materials
    [SerializeField] MeshRenderer[] model;
    [SerializeField] Material neutralMaterial;
    [SerializeField] Material selectedMaterial;

    // For Bouncing Projectiles
    [HideInInspector] public int maxBounces; // The maximum number of bounces before the projectile is destroyed. (Only applicable to Bouncing projectiles)
    [HideInInspector] public AudioClip bounceSound;
    [HideInInspector] public PhysicMaterial bouncyMaterial;
    public Transform mushroomObj; // The model of the object so we can rotate it upon each bounce
    private int numberOfBounces; // The number of times the projectile has bounced already.

    // For Homing Projectiles
    [HideInInspector] public float homingSpeed; // How fast the projectile can turn to home into a target
    [HideInInspector] public GameObject[] targets; // List of all players to determine the closest player
    //[HideInInspector] public GameObject mesh; // To store the model of the mushroom so when it bounces, it rotates the model accordingly
    private GameObject closestPlayer; // Variable that stores the closest player. (Only applicable toHoming projectiles)

    // For splash projectiles
    [HideInInspector] public float splashRadius; // The splash radius of the explosion.

    // Hidden Private Tags
    //private bool hasCollided; // Whether the projectile has collided with something or not.
    private Rigidbody rb;
    private bool isThrown;
    private Collider objCollider;
    private bool isSelected;

    // Hidden Public Tags
    [HideInInspector] public GameObject userThrowing; // The user holding and throwing the projectile
    [HideInInspector] public GameObject spawnZone; // The zone in which the projectile originates from
    

    private void OnEnable()
    {
        // Get the rigidbody, trailrenderer, and audioSource straight away
        // so you don't need to make them SerializeFields at the start and
        // clutter up everything.
        
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();
        objCollider = GetComponent<Collider>();

        trailRenderer.enabled = false;
    }

    private void Update()
    {
        // Checks from the player controller to see if this projectile IS the selectedProjectile...
        if (userThrowing != null)
        {
            if (userThrowing.TryGetComponent<PlayerController>(out PlayerController controller))
            {
                // If it IS selectedProjectile AND is not currently selected...
                // Transform all meshes to the selected material.
                if (controller.selectedProjectile == this.gameObject && !isSelected)
                {
                    isSelected = true;
                    Debug.Log("is Selected");
                    foreach (MeshRenderer mesh in model)
                    {
                        mesh.material = selectedMaterial;
                    }
                }
                else if (controller.selectedProjectile != gameObject && isSelected)
                {
                    isSelected = false;
                    Debug.Log("isn't selected");
                    foreach (MeshRenderer mesh in model)
                    {
                        mesh.material = neutralMaterial;
                    }
                }
            }

            // If the projectile is a homing projectile and is thrown, find the closest player every frame 
            if (projectileType == projectileBehaviour.homing && isThrown)
            {
                targets = GameObject.FindGameObjectsWithTag("Player");
                closestPlayer = ClosestPlayer();
            }
        }
    }

    // Method to find the nearest player with the 'player' tag
    GameObject ClosestPlayer()
    {
        GameObject closestHere = gameObject;
        float leastDistance = Mathf.Infinity;

        foreach (var player in targets)
        {
            if (player != userThrowing)
            {
                float distanceHere = Vector3.Distance(transform.position, player.transform.position);

                if (distanceHere < leastDistance)
                {
                    leastDistance = distanceHere;
                    closestHere = player;
                }
            }
        }
        return closestHere;
    }
    
    private void OnDrawGizmos()
    {
        if (projectileType == projectileBehaviour.rebound)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
        }
    }

    public void Throw()
    {
        isThrown = true; // Universal thrown tag
        rb.isKinematic = false; // Remove kinematicity
        rb.useGravity = false; // Don't use gravity
        rb.constraints = RigidbodyConstraints.FreezePositionY; // To ensure consistent collisions, the Y position is locked
        objCollider.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Projectiles"); // Changes the layer to the 'Projectile' layer so it doesn't collide with shit on the floor
        animator.SetBool("Throwing", true); // Initiate the throwing animation.

        // Enable the trail renderer and grab the player's player colour to tint it
        trailRenderer.enabled = true;
        trailRenderer.startColor = userThrowing.GetComponent<PlayerHealth>().playerColor;
        trailRenderer.endColor = userThrowing.GetComponent<PlayerHealth>().playerColor;

        // If the projectile isn't a homing projectile, apply an impulse
        if(projectileType != projectileBehaviour.homing)
        {
            rb.AddForce(transform.forward * thrownSpeed, ForceMode.Impulse);

            // If the projectile is a bouncy one, apply the bouncy physics material when thrown
            if (projectileType == projectileBehaviour.rebound)
            {
                //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                objCollider.material = bouncyMaterial;
            }
        }
    }
    private void FixedUpdate()
    {
        if (isThrown && projectileType == projectileBehaviour.homing)
        {
            Vector3 homingDirection = (closestPlayer.transform.position - rb.position).normalized;

            // Calculate the rotation to gradually face the target using cross product
            Quaternion targetRotation = Quaternion.LookRotation(homingDirection);
            Vector3 rotationAxis = Vector3.Cross(transform.forward, homingDirection);
            float rotationAngle = Vector3.Angle(transform.forward, homingDirection);

            // Calculate rotation step based on rotation speed
            float step = homingSpeed * Time.deltaTime;

            // Apply rotation along the calculated axis using Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step / rotationAngle);

            // Move the projectile towards the target
            rb.velocity = transform.forward * thrownSpeed;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (isThrown)
        {
            // Universal code if the projectile hits a player
            if (col.gameObject.tag == "Player")
            {
                col.gameObject.GetComponent<PlayerHealth>().OnHit();
                col.gameObject.GetComponent<PlayerInputHandler>().OnHit();

                numberOfBounces = 0;

                if (col.gameObject.GetComponent<PlayerHealth>().playerHealth <= 0 && col.gameObject != userThrowing)
                {
                    userThrowing.GetComponent<PlayerHealth>().killEffect.Play();
                    userThrowing.GetComponent<PlayerHealth>().kills += 1;
                }

                DefaultDestroy();
            }

            //Code for splash projectiles specifically
            else if (projectileType == projectileBehaviour.splash)
            {
                Debug.Log("Splash Projectile Code");
                Collider[] colliders = Physics.OverlapSphere(transform.position, splashRadius);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.tag == "Player")
                    {
                        collider.gameObject.GetComponent<PlayerHealth>().OnHit();
                        collider.gameObject.GetComponent<PlayerInputHandler>().OnHit();
                    }
                }
                DefaultDestroy();
            }

            // Code for bouncing projectiles specifically
            else if (col.gameObject.tag != "Player" && projectileType == projectileBehaviour.rebound)
            {
                if (numberOfBounces < maxBounces)
                {
                    numberOfBounces++;
                    animator.SetTrigger("Collision");
                    rb.angularVelocity = Vector3.zero; // So that the rotation of the rigidbody doesn't influence the bounce direction
                    mushroomObj.rotation = Quaternion.LookRotation(rb.velocity.normalized);
                    

                    if (numberOfBounces == 1)
                    {
                        audioSource.pitch = 0.6f;
                    }
                    else
                    {
                        // Increase the pitch by 0.1 after each bounce
                        audioSource.pitch += 0.1f;
                    }
                    audioSource.PlayOneShot(bounceSound, 0.5f);
                }
                else
                {
                    DefaultDestroy();
                }
            }

            // Unviersal code if the projectile hits a wall
            else
            {
                DefaultDestroy();
            }
        }

    }

    public void DefaultDestroy()
    {
        isThrown = false;
        rb.velocity = Vector3.zero; // Set the projectile velocity to 0
        rb.angularVelocity = Vector3.zero;
        Destroy(objCollider); // Destroy the collider

        spawnZone.GetComponent<FoodSpawner>().spawnedProjectiles.Remove(gameObject);

        foreach (MeshRenderer mesh in model) // Just in case it has two meshes
        {
            mesh.enabled = false;
        }

        // Play the splat sound at a random pitch & volume, play the particle effect
        float randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        float randomVolume = Random.Range(0.9f, 1.1f);
        audioSource.volume = randomVolume;
        audioSource.PlayOneShot(splatSound, randomVolume);
        splatParticle.Play();

        Destroy(gameObject, splatSound.length);
    }
}
