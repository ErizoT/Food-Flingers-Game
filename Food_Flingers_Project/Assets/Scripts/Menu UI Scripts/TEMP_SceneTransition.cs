using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// WILL REMOVE THIS LATER
// This is just a duct tape bridge just to ship mid term milestone out the door
// Just to transition from the main menu to the game.

public class TEMP_SceneTransition : MonoBehaviour
{
    public string targetSceneName;

    public void LoadScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
