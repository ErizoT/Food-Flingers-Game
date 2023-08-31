using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector2 rectangleBottomLeft; // Bottom-left corner of the rectangle
    public Vector2 rectangleTopRight;   // Top-right corner of the rectangle

    [HideInInspector] public GameObject respawnManager;
    private bool startSpawning;

    public Vector3[] points; // Used to determine the points for the debug gizmos

    [SerializeField] int spawnLimit = 10;
    [SerializeField] int spawnCount = 0;
    public List<GameObject> spawnedProjectiles;

    // Changing the fucking thing
    // Have a list of GameObjects (4 to be specific)
    // Get their transform.poitions

    [SerializeField] GameObject[] cornerPoints;

    private void Start()
    {
        respawnManager = GameObject.Find("Respawn Manager");
        //spawnedProjectiles = new GameObject[spawnLimit];
    }

    private void Update()
    {
        if (respawnManager.GetComponent<RespawnManager>().isGameStarted == true &&
            !startSpawning)
        {
            startSpawning = true;
            StartCoroutine(spawnLoop());
        }

        spawnCount = spawnedProjectiles.Count;
    }

    void OnDrawGizmos()
    {
        float rectangleHeight = transform.position.y; // Will remove later, want to adjust it with the gameobject instead of hardcoded.
        points = new Vector3[4]
        {
            new Vector3(cornerPoints[0].transform.position.x, rectangleHeight, cornerPoints[0].transform.position.z),
            new Vector3(cornerPoints[1].transform.position.x, rectangleHeight, cornerPoints[1].transform.position.z),
            new Vector3(cornerPoints[2].transform.position.x, rectangleHeight, cornerPoints[2].transform.position.z),
            new Vector3(cornerPoints[3].transform.position.x, rectangleHeight, cornerPoints[3].transform.position.z),

            //new Vector3(rectangleBottomLeft.x, rectangleHeight, rectangleBottomLeft.y), // Bottom Left
            //new Vector3(rectangleTopRight.x, rectangleHeight, rectangleTopRight.y),     // Top Right
            //new Vector3(rectangleBottomLeft.x, rectangleHeight, rectangleTopRight.y),
            //new Vector3(rectangleTopRight.x, rectangleHeight, rectangleBottomLeft.y),
        };

        // Draws two parallel blue lines
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(points[0], .5f);
        Gizmos.DrawSphere(points[1], .5f);
        Gizmos.DrawSphere(points[2], .5f);
        Gizmos.DrawSphere(points[3], .5f);

        Gizmos.DrawLine(points[0], points[2]);
        Gizmos.DrawLine(points[2], points[1]);
        Gizmos.DrawLine(points[1], points[3]);
        Gizmos.DrawLine(points[3], points[0]);
    }

    // Spawning Shit
    // - Foods spawned is kept in an array
    // - Spawn food if spawn count < spawn limit
    // - Add spawned food into the array

    public void SpawnRandomObject()
    {
       if (spawnCount < spawnLimit)
       {
            GameObject instantiatedObject;

            float randomX = Random.Range(rectangleBottomLeft.x, rectangleTopRight.x);
            float randomY = Random.Range(rectangleBottomLeft.y, rectangleTopRight.y);
            Vector3 randomPosition = new Vector3(randomX, transform.position.y, randomY);
            instantiatedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
            instantiatedObject.GetComponent<ProjectileBehaviour>().spawnZone = this.gameObject;
            spawnedProjectiles.Add(instantiatedObject);
            spawnCount++;
       }
    }

    IEnumerator spawnLoop()
    {
        SpawnRandomObject();
        yield return new WaitForSeconds(2f);
        StartCoroutine(spawnLoop());
    }
}
