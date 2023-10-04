using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration playerConfig;
    private PlayerController controller;
    private PlayerHealth healthController;
    [SerializeField] private SkinnedMeshRenderer playerMesh; // To be changed

    // User configured Material here

    [Header("AnimationStuff")]
    [SerializeField] Animator animator;
    [SerializeField] string holdingBoolName;
    [SerializeField] string runningBoolName;
    [SerializeField] string throwingBoolName;
    [SerializeField] string hurtBoolName;
    [SerializeField] string deadBoolName;

    private NewInput controls;

    [Header("SoundLibrary")]
    [SerializeField] AudioSource footstepSound;
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        controls = new NewInput();
        healthController = GetComponent<PlayerHealth>();
    }

    public void InitialisePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        playerMesh.material = pc.PlayerMaterial; // Will be changed to Outline colour in full version
        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    }

    private void Input_onActionTriggered(InputAction.CallbackContext obj)
    {
        if(obj.action.name == controls.Player.Move.name)
        {
            OnMove(obj);
        }

        if (obj.action.name == controls.Player.Fire.name && obj.performed)
        {
            Debug.Log("is firing");
            OnFire(obj);
        }

        if (obj.action.name == controls.Player.Jump.name && obj.performed)
        {
            OnDash(obj);
        }
    }

    public void Respawn()
    {
        animator.SetBool(deadBoolName, false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        controller.movementInput = context.ReadValue<Vector2>();
        animator.SetBool(runningBoolName, true);
        footstepSound.Play();

        if (controller.movementInput.magnitude < 0.1f)
        {
            animator.SetBool(runningBoolName, false);
            footstepSound.Stop();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(controller.isHolding)
        {
            controller.Throw();
            animator.SetTrigger(throwingBoolName);
            animator.SetBool(holdingBoolName, false);
        }
        else if (controller.selectedProjectile != null)
            {
            controller.Hold();
            animator.SetBool(holdingBoolName, true);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        controller.OnDash();
        animator.SetTrigger(throwingBoolName);
        animator.SetBool(holdingBoolName, false);
    }

    public void OnHit()
    {
        if(healthController.playerHealth <= 0)
        {
            animator.SetBool(deadBoolName, true);
            animator.SetBool(holdingBoolName, false);
        } else
        {
            animator.SetTrigger(hurtBoolName);
        }
    }
}
