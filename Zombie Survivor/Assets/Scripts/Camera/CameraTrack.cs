using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    // Create a list of all the targets to...target...with the camera 
    public GameObject[] targets;

    // Allows us to offset the position of the camera sicne the camera
    // will be directly inside our players.
    public Vector3 offset;

    public void Update()
    {
        // Constantly find any object with the tag 'Player' and make a targets list out of them
        targets = GameObject.FindGameObjectsWithTag("Player");
    }

    // We use LateUpdate because we're extracting the final positions
    // of players as part of the camera calculation, which are done in the
    // Update function. This stuff needs to come after it.
    private void LateUpdate()
    {
        // If the game does not detect any players, just return a null value
        if (targets.Length == 0)
        {
            return;
        }

        // We want the camera to occupy the center of all the targets
        Vector3 centerPoint = GetCenterPoint();

        // Add offset to the centerpoint
        Vector3 newPosition = centerPoint + offset;

        // Apply new position to camera position
        transform.position = newPosition;
    }

    // Returns a Vector3 | Function name is GetCenterPoint
    Vector3 GetCenterPoint()
    {
        // Checks if the number of targets is equal to one
        // If so, calculations dont matter, and defaults tracks to the one player.
        if (targets.Length == 1)
        {
            // Returns the position of the first player in the targets list
            return targets[0].transform.position;
        }

        // Unity 
        var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
        for (int i = 0; i < targets.Length; i++)
        {
            bounds.Encapsulate(targets[i].transform.position);
        }

        return bounds.center;
    }
}
