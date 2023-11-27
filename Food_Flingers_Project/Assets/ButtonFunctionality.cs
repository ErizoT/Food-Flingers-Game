using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonFunctionality : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip confirmSound;

    [Range(0.1f, 1.0f)]
    [SerializeField] float buttonVolume = 0.5f;

    [SerializeField] PlayerSetupMenuController setupMenu;
    [SerializeField] Color playerBackgroundColour;
    
    public void OnHover()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(hoverSound, buttonVolume);
    }

    public void OnSelect()
    {   
        Debug.Log("is selected");
        audioSource.PlayOneShot(confirmSound, buttonVolume);
    }

    public void FuckingColour()
    {
        //gotta do this cus the fuckign colour wont work from the playersetupmenucontroller for some dumb shit reason
        setupMenu.SetColor(playerBackgroundColour);
    }

    public void OnQuit()
    {
        // Find the object by name
        GameObject playerConfigManager = GameObject.Find("PlayerConfigurationManager");

        // Check if the object was found
        if (playerConfigManager != null)
        {
            SceneTransition.inGame = false;
            // Destroy the object
            Destroy(playerConfigManager);
        }
        else
        {
            // Object with the specified name was not found
            Debug.LogWarning("PlayerConfigurationManager not found.");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
