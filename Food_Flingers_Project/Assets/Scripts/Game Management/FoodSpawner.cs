using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Vector2 rectangleBottomLeft; // Bottom-left corner of the rectangle
    public Vector2 rectangleTopRight;   // Top-right corner of the rectangle

    private void Start()
    {
        Debug.Log("spawnloop started");
        StartCoroutine(spawnLoop());
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomObject();
        }
    }*/

    public void SpawnRandomObject()
    {
        GameObject respawnManager = GameObject.Find("Respawn Manager");
        if (respawnManager.GetComponent<RespawnManager>().isGameStarted == true)
        {
            float randomX = Random.Range(rectangleBottomLeft.x, rectangleTopRight.x);
            float randomY = Random.Range(rectangleBottomLeft.y, rectangleTopRight.y);
            Vector3 randomPosition = new Vector3(randomX, 1f, randomY);
            Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        }
    }

    IEnumerator spawnLoop()
    {
        
        Debug.Log("spawning");
        yield return new WaitForSeconds(2f);
        SpawnRandomObject();
        StartCoroutine(spawnLoop());
    }
}
