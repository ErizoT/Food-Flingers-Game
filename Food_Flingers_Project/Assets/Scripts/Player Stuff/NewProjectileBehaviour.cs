using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int numberOfBounces; // The number of times the projectile has bounced already.

    // For Homing Projectiles
    [HideInInspector] public float homingSpeed; // How fast the projectile can turn to home into a target
    [HideInInspector] public GameObject[] targets; // List of all players to determine the closest player
    private GameObject closestPlayer; // Variable that stores the closest player. (Only applicable toHoming projectiles)

    // For splash projectiles
    [HideInInspector] public float splashRadius; // The splash radius of the explosion.

    // Hidden Private Tags
    private bool hasCollided; // Whether the projectile has collided with something or not.
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
        if (userThrowing.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            // If it IS selectedProjectile AND is not currently selected...
            // Transform all meshes to the selected material.
            if (controller.selectedProjectile == this.gameObject && !isSelected)
            {
                isSelected = true;
                foreach (MeshRenderer mesh in model)
                {
                    mesh.material = selectedMaterial;
                }
            }
            else if (controller.selectedProjectile != gameObject && isSelected)
            {
                isSelected = false;
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

    public void Throw()
    {
        isThrown = true; // Universal thrown tag
        rb.isKinematic = false; // Remove kinematicity
        rb.useGravity = false; // Don't use gravity
        rb.constraints = RigidbodyConstraints.FreezePositionY; // To ensure consistent collisions, the Y position is locked

        LayerMask.NameToLayer("Projectiles"); // Changes the layer to the 'Projectile' layer so it doesn't collide with shit on the floor
        animator.SetBool("Throwing", true); // Initiate the throwing animation.

        // Enable the trail renderer and grab the player's player colour to tint it
        trailRenderer.enabled = true;
        trailRenderer.startColor = userThrowing.GetComponent<PlayerHealth>().playerColor;
        trailRenderer.endColor = userThrowing.GetComponent<PlayerHealth>().playerColor;

        // If the projectile isn't a homing projectile, apply an impulse
        if(projectileType != projectileBehaviour.homing)
        {
            rb.AddForce(transform.forward * thrownSpeed, ForceMode.Impulse);
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
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().OnHit();
            col.gameObject.GetComponent<PlayerInputHandler>().OnHit();

            numberOfBounces = 0;

            if (col.gameObject.GetComponent<PlayerHealth>().playerHealth <= 0 && col.gameObject != userThrowing)
            {
                //Debug.Log(userThrowing + " awarded a kill");
                userThrowing.GetComponent<PlayerHealth>().kills += 1;
            }

            DefaultDestroy();
        }
    }

    private void DefaultDestroy()
    {
        rb.velocity = Vector3.zero; // Set the projectile velocity to 0
        Destroy(objCollider); // Destroy the collider

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
