using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FoodSpawner : MonoBehaviour
{
    [SerializeField] ScriptExercise foodArray;
    private Vector2 rectangleBottomLeft; // Bottom-left corner of the rectangle
    private Vector2 rectangleTopRight;   // Top-right corner of the rectangle

    [HideInInspector] public GameObject respawnManager;
    private bool startSpawning;

    public Vector3[] points; // Used to determine the points for the debug gizmos

    [SerializeField] int spawnLimit = 10;
    [SerializeField] int spawnCount = 0;
    public List<GameObject> spawnedProjectiles;

    // Changing the fucking thing
    // Have a list of GameObjects (4 to be specific)
    // Get their transform.poitions

    public GameObject[] cornerPoints;

    private void Start()
    {
        respawnManager = GameObject.Find("Respawn Manager");

        rectangleBottomLeft = new Vector2(cornerPoints[2].transform.position.x, cornerPoints[2].transform.position.z);
        rectangleTopRight = new Vector3(cornerPoints[1].transform.position.x, cornerPoints[1].transform.position.z);
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

        //Debug.Log(startSpawning);
    }

    void OnDrawGizmos()
    {
        float rectangleHeight = transform.position.y; // Will remove later, want to adjust it with the gameobject instead of hardcoded.
        points = new Vector3[4]
        {
            new Vector3(cornerPoints[0].transform.position.x, rectangleHeight, cornerPoints[0].transform.position.z), // Top Left Corner
            new Vector3(cornerPoints[1].transform.position.x, rectangleHeight, cornerPoints[1].transform.position.z), //Top Right Corner
            new Vector3(cornerPoints[2].transform.position.x, rectangleHeight, cornerPoints[2].transform.position.z), // Bottom Left Corner
            new Vector3(cornerPoints[3].transform.position.x, rectangleHeight, cornerPoints[3].transform.position.z), // Bottom Right Corner
        };

        // Draws two parallel blue lines
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(points[0], .5f);
        Gizmos.DrawSphere(points[1], .5f);
        Gizmos.DrawSphere(points[2], .5f);
        Gizmos.DrawSphere(points[3], .5f);

        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[3]);
        Gizmos.DrawLine(points[3], points[2]);
        Gizmos.DrawLine(points[2], points[0]);
    }

    // Spawning Shit
    // - Foods spawned is kept in an array
    // - Spawn food if spawn count < spawn limit
    // - Add spawned food into the array

    public void SpawnRandomObject()
    {
       if (spawnCount < spawnLimit) // Checks to see if the number of projectiles spawned is less than the limit
       {
            float totalProbability = 0f;
            foreach (float probability in foodArray.LikelihoodList)
            {
                totalProbability += probability;
            }

            float randomValue = Random.Range(0f, totalProbability);
            float cumulativeProbability = 0f;

            for (int i = 0; i < foodArray.FoodList.Count; i++)
            {
                cumulativeProbability += foodArray.LikelihoodList[i];
                if (randomValue < cumulativeProbability)
                {
                    GameObject instantiatedObject;

                    float randomX = Random.Range(rectangleBottomLeft.x, rectangleTopRight.x);   // Get a random X position
                    float randomY = Random.Range(rectangleBottomLeft.y, rectangleTopRight.y);   // Get a random Y position
                    Vector3 randomPosition = new Vector3(randomX, transform.position.y, randomY);   // Get those random XYs and turn them into random position
                    instantiatedObject = Instantiate(foodArray.FoodList[i], randomPosition, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));   // Instantiate that random object according to probbability
                    instantiatedObject.GetComponent<NewProjectileBehaviour>().spawnZone = this.gameObject; // Assign the projectile's spawn zone
                    spawnedProjectiles.Add(instantiatedObject); // Add the projectile to the spawnedProjectiles list
                    spawnCount++;   // Add to the spawn zone's spawncount
                    break;
                }
            }
       }
    }

    IEnumerator spawnLoop()
    {
        SpawnRandomObject();
        yield return new WaitForSeconds(2f);
        StartCoroutine(spawnLoop());
    }
}
