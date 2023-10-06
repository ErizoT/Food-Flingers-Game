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
}
