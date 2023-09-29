using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetSceneName;
    [SerializeField] Animator anim;
    [SerializeField] bool animated;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "PlayerSetup")
        {
            animated = true;
        }
    }

    public void LoadScene()
    {
        if (animated && anim != null)
        {
            StartCoroutine(Transition());
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    IEnumerator Transition()
    {
        anim.SetInteger("transitionState", 1);
        yield return new WaitForSeconds(2.5f); // Example delay of 1 second

        SceneManager.LoadScene(targetSceneName);
        yield return new WaitForSeconds(1f); // Example delay of 1 second

        anim.SetInteger("transitionState", 2);
        yield return new WaitForSeconds(2f); // Example delay of 1 second

        anim.SetInteger("transitionState", 0);

        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            GameObject respawnManager = GameObject.Find("Respawn Manager");
            respawnManager.GetComponent<RespawnManager>().InitialiseGame();
        }

    }
}
