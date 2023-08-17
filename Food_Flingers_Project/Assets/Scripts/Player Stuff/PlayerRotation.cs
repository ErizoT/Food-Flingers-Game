using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private string tagToDetect = "Enemy";
    public GameObject[] enemyList;
    public GameObject closestEnemy;



    // Start is called before the first frame update
    void Start()
    {
        //enemyList = GameObject.FindGameObjectsWithTag(tagToDetect);
    }

    // Update is called once per frame
    void Update()
    {
        enemyList = GameObject.FindGameObjectsWithTag(tagToDetect);
        closestEnemy = ClosestEnemy();
        transform.LookAt(closestEnemy.transform);
        //Debug.Log(closestEnemy);
    }

    GameObject ClosestEnemy()
    {
        GameObject closestHere = gameObject;
        float leastDistance = Mathf.Infinity;

        foreach(var enemy in enemyList)
        {
            float distanceHere = Vector3.Distance(transform.position, enemy.transform.position);

            if(distanceHere < leastDistance)
            {
                leastDistance = distanceHere;
                closestHere = enemy;
            }
        }

        return closestHere;
    }
}
