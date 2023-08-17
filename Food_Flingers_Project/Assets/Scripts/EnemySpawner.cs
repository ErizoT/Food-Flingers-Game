using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject muncherObject;
    [SerializeField] float spawnTime = 2f;
    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        //Vector3 randomSpot = Vector3(transform.position.x, transform.position.y, Random.Range(-10f, 10f))
    }

    IEnumerator SpawnLoop()
    {
        Object.Instantiate(muncherObject, transform.position + new Vector3(transform.position.x, transform.position.y, Random.Range(-10f, 10f)), transform.rotation);
        yield return new WaitForSeconds(spawnTime);
        StartCoroutine(SpawnLoop());
    }
}
