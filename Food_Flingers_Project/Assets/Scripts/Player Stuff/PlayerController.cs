using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Bunch of movement values I can adjust
    public float playerSpeed = 11f;
    [SerializeField] private float rotationInterpolation = 10f;

    public Rigidbody rb;

    public bool canMove = true;

    // The variable that contains the character controller I had assigned at the start
    private CharacterController controller;

    // playerVelocity is essentially just the movement of the player
    private Vector3 playerVelocity;

    // Boolean that determines whether a player is on the ground
    private bool groundedPlayer;

    // the two-axis variable that tracks the player input of X and Y
    private Vector2 movementInput = Vector2.zero;

    // Raycast Stuff
    [SerializeField] LayerMask projectiles;
    

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Call Spherecast on update
        // Object hit by raycast will change colour to green | otherwise will change back to orange
        // Upon grabbing (OnGrab)...
        // - If holding a food already, throw the food
        // - If not holding food, pick up

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 3f, projectiles))
        {
            hit.transform.gameObject.GetComponent<MeshRenderer>
        }
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        //controller.Move(move * Time.deltaTime * playerSpeed);

        // The force is both the direction AND the magnitude (both lenght and direction)
        GetComponent<Rigidbody>().AddForce(move * playerSpeed * 10f, ForceMode.Force);

        if (move.magnitude != 0)
        {
            // Rotating the player to the direction they are inputting movement
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(move),Time.deltaTime*rotationInterpolation);
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        //Shit that happens when moving
        if (canMove)
        {
            movementInput = context.ReadValue<Vector2>();
        }
    }

    public void OnGrab()
    {

    }
}