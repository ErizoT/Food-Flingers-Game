using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footstepAnims : MonoBehaviour
{
    public AudioSource sauce;

    public void PlayFootstep()
    {
        sauce.pitch = Random.Range(0.7f, 1.3f);
        sauce.Play();
    }
}
