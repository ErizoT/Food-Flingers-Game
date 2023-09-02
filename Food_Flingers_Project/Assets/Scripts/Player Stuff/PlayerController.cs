using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Bunch of movement values I can adjust
    public float playerSpeed = 11f;
    [SerializeField] float invulnerabilityTime = 0.5f;
    [SerializeField] private float rotationInterpolation = 10f;
    

    private Rigidbody rb;
    public bool canMove = true;

    [SerializeField] Material defMat;
    [SerializeField] Material transpMat;

    // the two-axis variable that tracks the player input of X and Y
    private Vector2 movementInput = Vector2.zero;
    private bool dashing = false;

    // Raycast Stuff
    [SerializeField] LayerMask projectiles;

    // Throwing Stuff and Hitbox Stuff
    public GameObject selectedProjectile = null;
    [SerializeField] GameObject heldProjectile;
    [SerializeField] float sphereRadius = 2f;
    [SerializeField] float hitboxDistance = 3f;
    private bool isHolding;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        projectiles = LayerMask.GetMask("Items");
    }
    

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        //controller.Move(move * Time.deltaTime * playerSpeed);

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

        // Call Spherecast on update
        // Object hit by raycast will change colour to green | otherwise will change back to orange
        // Upon grabbing (OnGrab)...
        // - If holding a food already, throw the food
        // - If not holding food, pick up

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, hitboxDistance, projectiles))
        {
            selectedProjectile = hit.transform.gameObject;
            selectedProjectile.GetComponent<ProjectileBehaviour>().userThrowing = this.gameObject;
        }
        else
        {
            selectedProjectile = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.5f); // Blue color with 50% opacity
        Vector3 spherePosition = transform.position + transform.forward * hitboxDistance;
        Gizmos.DrawSphere(spherePosition, sphereRadius);
        Gizmos.DrawLine(transform.position, spherePosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Shit that happens when moving
        if (canMove)
        {
            movementInput = context.ReadValue<Vector2>();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !dashing && !isHolding)
        {
            dashing = true;
            StartCoroutine(Dash());
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isHolding)
            {
                Throw();
            }
            else
            {
                Hold();
            }
        }
    }

    void Hold()
    {

        if (selectedProjectile != null)
        {
            heldProjectile = selectedProjectile;

            Transform foodTransform = heldProjectile.transform;

            foodTransform.SetParent(transform);
            foodTransform.position = transform.position + new Vector3(0, 2, 0);
            foodTransform.rotation = transform.rotation;
            heldProjectile.GetComponent<Rigidbody>().isKinematic = true;
            heldProjectile.GetComponent<SphereCollider>().enabled = false;

            isHolding = true;
        } 
        else
        {
            return;
        }
    }

    void Throw()
    {
        if (heldProjectile != null)
        {
            heldProjectile.transform.SetParent(null);
            heldProjectile.transform.position = this.transform.position + transform.forward * 2;
            heldProjectile.GetComponent<SphereCollider>().enabled = true;
            heldProjectile.GetComponent<Rigidbody>().isKinematic = false;
            heldProjectile.GetComponent<Rigidbody>().useGravity = false;
            heldProjectile.GetComponent<ProjectileBehaviour>().isThrown = true;

            heldProjectile = null;
            isHolding = false;
        }
            
    }

    IEnumerator Dash()
    {
        Debug.Log("Dashing...");
        
        Vector3 dashDirection = transform.forward; // Calculate the dash direction (e.g., forward).
        float dashForce = 50f;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse); // Apply the dash force to the player's Rigidbody.
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        GetComponent<MeshRenderer>().material = transpMat;

        yield return new WaitForSeconds(invulnerabilityTime);

        GetComponent<MeshRenderer>().material = defMat;
        gameObject.layer = LayerMask.NameToLayer("Default"); // Reset the player's layer
        dashing = false;
    }
}