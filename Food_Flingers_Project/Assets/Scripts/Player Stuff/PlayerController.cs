using System.Collections;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Bunch of movement values I can adjust
    [Header("Movement")]
    public float playerSpeed = 11f;
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] private float rotationInterpolation = 10f;
    [SerializeField] float dashForce = 80f;
    public bool canMove = true;

    private Rigidbody rb; // This variable was missing from the original response

    [HideInInspector] public Vector2 movementInput = Vector2.zero;
    private bool dashing = false;

    // Raycast Stuff
    [HideInInspector] LayerMask projectiles;

    [Header("Spherecast Stuff")]
    [SerializeField] Vector3 offset;
    private Vector3 raycastStartPos;
    [SerializeField] float sphereRadius = 2f;
    [SerializeField] float hitboxDistance = 3f;
    [HideInInspector] public GameObject selectedProjectile = null;
    GameObject heldProjectile;
    [HideInInspector] public bool isHolding;

    [Header("Pause Menu Variables")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Button pauseButton;
    [SerializeField] SkinnedMeshRenderer rend;
    public static bool isPaused = false;
    [HideInInspector] public static GameObject playerPaused;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        projectiles = LayerMask.GetMask("Items");

        // Detect other players in the scene by tag
        GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Check the number of other players
        int numberOfOtherPlayers = otherPlayers.Length;
    }
    

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;


        // The force is both the direction AND the magnitude (both lenght and direction)
        if (!dashing)
        {
            GetComponent<Rigidbody>().AddForce(move * playerSpeed * 10f, ForceMode.Force);
        }

        if (move.magnitude != 0)
        {
            // Rotating the player to the direction they are inputting movement
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(move),Time.deltaTime*rotationInterpolation);
        }

        // Spherecasting item pickup range
        RaycastHit hit;
        raycastStartPos = transform.position + offset;

        if (Physics.SphereCast(raycastStartPos, sphereRadius, transform.forward, out hit, hitboxDistance, projectiles))
        {
            selectedProjectile = hit.transform.gameObject;
            selectedProjectile.GetComponent<ProjectileBehaviour>().userThrowing = this.gameObject;
        }
        else
        {
            selectedProjectile = null;
            //StartCoroutine(CoyoteGrab());     - See notes under CoyoteGrab() below
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.5f); // Blue color with 50% opacity

        Vector3 raycastStartPos = transform.position + offset;

        Vector3 spherePosition = raycastStartPos + transform.forward * hitboxDistance;
        Gizmos.DrawSphere(spherePosition, sphereRadius);
        Gizmos.DrawLine(raycastStartPos, spherePosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Shit that happens when moving
        if (canMove)
        {
            //movementInput = context.ReadValue<Vector2>();
        }
        else
        {
            return;
        }
    }

    public void OnDash()
    {
        if (!dashing && rb != null && canMove && !isPaused)
        {
            dashing = true;
            StartCoroutine(Dash());

            if (isHolding)
            {
                Drop();
            }
        }
    }

    public void Hold()
    {

        if (selectedProjectile != null)
        {
            heldProjectile = selectedProjectile;

            Transform foodTransform = heldProjectile.transform;

            foodTransform.SetParent(transform);
            foodTransform.position = transform.position + new Vector3(0, 3, 0);
            foodTransform.rotation = transform.rotation;
            heldProjectile.GetComponent<Rigidbody>().isKinematic = true;
            heldProjectile.GetComponent<CapsuleCollider>().enabled = false;

            heldProjectile.GetComponent<ProjectileBehaviour>().animator.SetBool("Holding", true);

            isHolding = true;
        } 
        else
        {
            return;
        }
    }
    public void Throw()
    {
        if (heldProjectile != null)
        {
            heldProjectile.transform.SetParent(null);
            heldProjectile.transform.position = raycastStartPos + transform.forward * 2;
            heldProjectile.GetComponent<CapsuleCollider>().enabled = true;
            heldProjectile.GetComponent<Rigidbody>().isKinematic = false;
            heldProjectile.GetComponent<Rigidbody>().useGravity = false;
            heldProjectile.GetComponent<ProjectileBehaviour>().isThrown = true;

            heldProjectile = null;
            isHolding = false;
        }
            
    }
    public void Drop()
    {
        if (isHolding)
        {
            heldProjectile.transform.SetParent(null);
            heldProjectile.GetComponent<CapsuleCollider>().enabled = true;
            heldProjectile.GetComponent<Rigidbody>().isKinematic = false;
            heldProjectile = null;
            isHolding = false;
        }
    }





    // Helper method to change the player's color
    private void ChangeColor(Color newColor)
    {
        // Assuming you have a MeshRenderer component on your player object
        //MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;
        mat.color = newColor;
    }

    IEnumerator Dash()
    {
        Debug.Log("Dashing...");

        Vector3 dashDirection = transform.forward; // Calculate the dash direction (e.g., forward).
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse); // Apply the dash force to the player's Rigidbody.
        //gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        //MeshRenderer rend = GetComponent<MeshRenderer>();
        //Material mat = rend.material;
        //Color matColor = mat.color;
        //matColor.a = 0.5f;
        //mat.color = matColor;

        yield return new WaitForSeconds(invulnerabilityTime);

        //matColor.a = 1f;
        //mat.color = matColor;

        //gameObject.layer = LayerMask.NameToLayer("Default"); // Reset the player's layer
        dashing = false;
    }

    // ============================= MENU STUFF ====================================
    public void Pause()
    {
        if (isPaused && playerPaused == this.gameObject)
        {
            Unpause();
        }     
        isPaused = true;

        Time.timeScale = 0;
        playerPaused = this.gameObject;
        pauseMenu.SetActive(true);
        pauseButton.Select();
        // Play a pause sound here!

    }

    public void Unpause()
    {
        Debug.Log("Unpausing");
        isPaused = false;

        Time.timeScale = 1;
        playerPaused = null;
        pauseMenu.SetActive(false);

        // Play a pause sound here!
    }

    public void Exit()
    {
        Unpause();
        SceneManager.LoadScene("MainMenu");
    }
}