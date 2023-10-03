using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float projectileVelocity;
    [HideInInspector] public bool isThrown;
    [HideInInspector] public Rigidbody rb;
    public GameObject userThrowing;
    [HideInInspector] public GameObject spawnZone;
    public Animator animator;

    [SerializeField] Material neutralMaterial;
    [SerializeField] Material selectedMaterial;

    private Vector3 direction;
    private int numberOfBounces;
    private int maxBounces = 4; // Gonna make this configurable later

    // Homing projectile values
    public GameObject[] targets;
    private GameObject closestPlayer;
    [SerializeField] float rotationSpeed = 50f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] MeshRenderer[] meshes;
    private bool hasCollided;

    enum projectileBehaviour
    {
        straightforward,
        rebound,
        homing,
        splash
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
                //model.GetComponent<MeshRenderer>().material = selectedMaterial; // Doesn't work, would like to make a select shader
            }
            else
            {
                //model.GetComponent<MeshRenderer>().material = neutralMaterial;

                if (!isThrown)
                {
                    userThrowing = this.gameObject;
                }
            }
        }

        // Finding the closest player
        targets = GameObject.FindGameObjectsWithTag("Player");
        closestPlayer = ClosestPlayer();
    }

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

    // Drawing the debug forward line of direction of the projectile 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }

    private void FixedUpdate ()
    {
        if (isThrown && !hasCollided)
        {
            gameObject.layer = LayerMask.NameToLayer("Projectiles"); // Changes the layer to the 'Projectile' layer so it doesn't collide with shit on the floor
            animator.SetBool("Throwing", true);

            switch (projectileType)
            {
                case projectileBehaviour.straightforward:
                    rb.MovePosition(transform.position + transform.forward * projectileVelocity * Time.deltaTime);
                    break;

                case projectileBehaviour.rebound:
                    if (numberOfBounces == 0)
                    {
                        direction = transform.forward;
                    } 
                    else if (numberOfBounces == maxBounces)
                    {
                        DefaultDestroy();
                        return;
                    }

                    rb.MovePosition(transform.position + direction * projectileVelocity * Time.deltaTime);
                    break;

                case projectileBehaviour.homing:
                    Vector3 playerOffset = new Vector3(0, 1, 0); // This is an offset, because the 0,0 transform is on the floor as opposed to the middle of the body
                    Vector3 homingDirection = (closestPlayer.transform.position + playerOffset - rb.position).normalized;

                    // Calculate the rotation to gradually face the target using cross product
                    Quaternion targetRotation = Quaternion.LookRotation(homingDirection);
                    Vector3 rotationAxis = Vector3.Cross(transform.forward, homingDirection);
                    float rotationAngle = Vector3.Angle(transform.forward, homingDirection);

                    // Calculate rotation step based on rotation speed
                    float step = rotationSpeed * Time.deltaTime;

                    // Apply rotation along the calculated axis using Slerp
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step / rotationAngle);

                    // Move the projectile towards the target
                    rb.velocity = transform.forward * projectileVelocity;
                    break;

                // Add more switch cases here once more projectile behaviours are developed!

                default:
                    return;
            }
        }
    }

    public void OnCollisionEnter(Collision col)
    {

        if (isThrown && !hasCollided)
        {

            switch (projectileType)
            {
                case projectileBehaviour.straightforward:
                    hasCollided = true;
                    GetComponent<CapsuleCollider>().enabled = false;

                    foreach(MeshRenderer mesh in meshes) // Just in case it has two meshes
                    {
                        mesh.enabled = false;
                    }

                    DefaultDestroy();
                    break;

                case projectileBehaviour.rebound:
                    direction = Vector3.Reflect(direction, col.contacts[0].normal);
                    numberOfBounces++;
                    transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
                    Debug.Log(numberOfBounces);
                    break;

                case projectileBehaviour.homing:
                    DefaultDestroy();
                    break;

            }
        }

        if (isThrown && col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerHealth>().OnHit();
            col.gameObject.GetComponent<PlayerInputHandler>().OnHit();

            if (col.gameObject.GetComponent<PlayerHealth>().playerHealth <= 0 && col.gameObject != userThrowing)
            {
                Debug.Log(userThrowing + " awarded a kill");
                userThrowing.GetComponent<PlayerHealth>().kills += 1;
            }
            DefaultDestroy();
        }
        
    }

    public void DefaultDestroy()
    {
        float randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        audioSource.Play();
        spawnZone.GetComponent<FoodSpawner>().spawnedProjectiles.Remove(this.gameObject);
        Destroy(this.gameObject, audioSource.clip.length);
    }
}
