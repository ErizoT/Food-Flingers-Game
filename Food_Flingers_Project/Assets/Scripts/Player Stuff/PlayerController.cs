using UnityEngine;
using UnityEngine.InputSystem;

// Require component essentially adds the 'character controller' component to the gameobject
// upon the presence of this script. This is so that I don't need to painstakingly add a character
// controller everytime I use this script.
//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Bunch of movement values I can adjust
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationInterpolation = 10f;

    public Rigidbody rb;

    // The variable that contains the character controller I had assigned at the start
    private CharacterController controller;

    // playerVelocity is essentially just the movement of the player
    private Vector3 playerVelocity;

    // Boolean that determines whether a player is on the ground
    private bool groundedPlayer;

    // the two-axis variable that tracks the player input of X and Y
    private Vector2 movementInput = Vector2.zero;

    // Self explanatory
    private bool jumped = false;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        rb = this.GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Shit that happens when moving
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Shit that happens when jumping

        jumped = context.action.triggered;
    }

    void FixedUpdate()
    {
        /*groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }*/

        //Debug.Log(movementInput);

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        //controller.Move(move * Time.deltaTime * playerSpeed);

        // The force is both the direction AND the magnitude (both lenght and direction)
        GetComponent<Rigidbody>().AddForce(move * playerSpeed, ForceMode.Force);

        if (move.magnitude != 0)
        {
            // Rotating the player to the direction they are inputting movement
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(move),Time.deltaTime*rotationInterpolation);
        }
        

        /*if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }*/

        // Changes the height position of the player..
        /*if (jumped && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);*/
    }

}