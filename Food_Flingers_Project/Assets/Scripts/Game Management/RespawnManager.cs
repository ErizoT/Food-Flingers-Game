using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RespawnManager : MonoBehaviour
{
    public GameObject[] playerList;
    public int maxPlayers = 2;
    public Transform[] respawnPoints;
    public TextMeshProUGUI titleText;

    public bool isGameStarted;

    public void Update()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");

        if (playerList.Length == maxPlayers && !isGameStarted)
        {
            isGameStarted = true;
            InitialiseGame();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            InitialiseGame();
            GameObject[] foodToDestroy = GameObject.FindGameObjectsWithTag("Food");
            StartCoroutine(GameCountdown());

            for (int i = 0; i < foodToDestroy.Length; i++)
            {
                Destroy(foodToDestroy[i]);
            }
        }
    }

    void InitialiseGame()
    {
        Debug.Log("game started");

        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].transform.position = respawnPoints[i].position;
        }

        StartCoroutine(GameCountdown());
    }
    
    IEnumerator GameCountdown()
    {

        for (int i = 3; i >= 0; i--)
        {
            Debug.Log("Countdown: " + i);
            titleText.text = (i.ToString());
            yield return new WaitForSeconds(1f);

            if (i == 0)
            {
                titleText.text = ("FLING!");
                yield return new WaitForSeconds(1f);
                titleText.text = null;
            }
        }
    }
}
