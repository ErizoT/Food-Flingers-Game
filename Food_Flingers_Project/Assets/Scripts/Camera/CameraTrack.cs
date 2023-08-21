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
    public Vector3 rotationOffset;
    private Vector3 velocity;

    // The interpolation smoothness between positions on the camera
    public float smoothTime = .5f;

    // For zooming
    public float minZoom = 90f;
    public float maxZoom = 40f;
    public float zoomLimiter = 50f;

    //public Vector3 rotation;

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

        Move();
        Zoom();
    }
    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        this.GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, newZoom, Time.deltaTime);
    }

    void Move()
    {
        // We want the camera to occupy the center of all the targets
        Vector3 centerPoint = GetCenterPoint();

        // Add offset to the centerpoint
        Vector3 newPosition = centerPoint + offset;

        // Apply new position to camera position
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        transform.rotation = Quaternion.Euler(rotationOffset);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
        for (int i = 0; i < targets.Length; i++)
        {
            bounds.Encapsulate(targets[i].transform.position);
        }

        return bounds.size.x;
    }

    // Returns a Vector3 | Function name is GetCenterPoint
    Vector3 GetCenterPoint()
    {
        // Checks if the number of targets is equal to one.
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
