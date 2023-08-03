using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Muncher: MonoBehaviour
{
    [SerializeField] private string tagToDetect = "Player";
    public GameObject[] playerList;
    public GameObject closestPlayer;

    [SerializeField] private float movementSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        //enemyList = GameObject.FindGameObjectsWithTag(tagToDetect);
    }

    // Update is called once per frame
    void Update()
    {
        playerList = GameObject.FindGameObjectsWithTag(tagToDetect);
        closestPlayer = ClosestPlayer();
        transform.LookAt(closestPlayer.transform);

        // ------- Movement -------
        //Vector3 playerDirection = (closestPlayer.transform.position - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, closestPlayer.transform.position, movementSpeed * Time.deltaTime);

    }

    GameObject ClosestPlayer()
    {
        GameObject closestHere = gameObject;
        float leastDistance = Mathf.Infinity;

        foreach (var player in playerList)
        {
            float distanceHere = Vector3.Distance(transform.position, player.transform.position);

            if (distanceHere < leastDistance)
            {
                leastDistance = distanceHere;
                closestHere = player;
            }
        }

        return closestHere;
    }
}
