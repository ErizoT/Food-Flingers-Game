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

    [Header("Sound Library")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] throwSound;

    [SerializeField] AudioClip[] damageSound;

    [SerializeField] AudioClip[] knockoutSound;

    [SerializeField] AudioClip dashSound;

    [SerializeField] AudioClip pickupSound;

    [Header("Sound Volumes")]
    [Range(0.1f, 1f)]
    [SerializeField] float pickupVolume = 0.5f;
    [Range(0.1f, 1f)]
    [SerializeField] float dashVolume = 0.5f;    
    [Range(0.1f, 1f)]
    [SerializeField] float knockoutVolume = 0.5f;
    [Range(0.1f, 1f)]
    [SerializeField] float damageVolume = 0.5f;
    [Range(0.1f, 1f)]
    [SerializeField] float throwVolume = 0.5f;

    [Header("Effects Library")]
    [SerializeField] ParticleSystem smokeTrail;
    private bool trailExists;
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

        if (obj.action.name == controls.Player.Pause.name && obj.performed)
        {
            OnPause(obj);
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

        if (controller.movementInput.magnitude < 0.1f)
        {
            animator.SetBool(runningBoolName, false);
            StopSmokeTrail(); // Stop the smoke trail when not moving
        }
        else
        {
            PlaySmokeTrail();
        }
    }
    private void PlaySmokeTrail()
    {
        if (!trailExists)
        {
            smokeTrail.Play();
            trailExists = true;
        }
    }

    private void StopSmokeTrail()
    {
        if (trailExists)
        {
            smokeTrail.Stop();
            trailExists = false;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(controller.isHolding)
        {
            controller.Throw();
            animator.SetTrigger(throwingBoolName);
            animator.SetBool(holdingBoolName, false);

            audioSource.volume = throwVolume;
            audioSource.pitch = Random.Range(0.7f, 1.3f);
            audioSource.PlayOneShot(throwSound[Random.Range(0, throwSound.Length)]);
            
        }
        else if (controller.selectedProjectile != null)
            {
            controller.Hold();
            animator.SetBool(holdingBoolName, true);

            audioSource.volume = pickupVolume;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(pickupSound);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        controller.OnDash();
        animator.SetTrigger(throwingBoolName);
        animator.SetBool(holdingBoolName, false);

        audioSource.volume = dashVolume;
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.PlayOneShot(dashSound);
    }

    public void OnHit()
    {

        if(healthController.playerHealth <= 0)
        {
            animator.SetBool(deadBoolName, true);
            animator.SetBool(holdingBoolName, false);

            audioSource.volume = knockoutVolume;
            int r = Random.Range(0, knockoutSound.Length);
            audioSource.PlayOneShot(knockoutSound[r]);
        } else
        {
            audioSource.volume = damageVolume;
            int r = Random.Range(0, damageSound.Length);
            audioSource.PlayOneShot(damageSound[r]);
            animator.SetTrigger(hurtBoolName);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        
        controller.Pause();
    }
}
