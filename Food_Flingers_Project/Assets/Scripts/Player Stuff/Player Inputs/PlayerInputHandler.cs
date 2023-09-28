using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration playerConfig;
    private PlayerController controller;

    // User configured Material here

    private NewInput controls;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        controls = new NewInput();
    }

    public void InitialisePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        // Apply player material
        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    }

    private void Input_onActionTriggered(InputAction.CallbackContext obj)
    {
        if(obj.action.name == controls.Player.Move.name)
        {
            OnMove(obj);
        }

        if (obj.action.name == controls.Player.Fire.name)
        {
            OnFire(obj);
        }

        if (obj.action.name == controls.Player.Jump.name)
        {
            OnDash(obj);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        controller.movementInput = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        controller.OnGrab();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        controller.OnDash();
    }
}
