using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetSceneName;
    [SerializeField] Animator anim;

    public void LoadScene()
    {
        Debug.Log("Transitioning");
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        anim.SetInteger("transitionState", 1);
        yield return new WaitForSeconds(2f); // Example delay of 1 second

        SceneManager.LoadScene(targetSceneName);
        yield return new WaitForSeconds(1f); // Example delay of 1 second

        anim.SetInteger("transitionState", 2);
        yield return new WaitForSeconds(2f); // Example delay of 1 second

        anim.SetInteger("transitionState", 0);

    }
}
