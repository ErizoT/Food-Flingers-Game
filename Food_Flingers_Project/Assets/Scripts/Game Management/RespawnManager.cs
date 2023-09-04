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

    // Game Timers
    public float totalTime = 60f; // Total time for the countdown in seconds
    private float currentTime;
    private bool isCounting = false;
    [SerializeField] TextMeshProUGUI timerText;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();
    }

    public void Update()
    {
        // Initialising player list and starting when players match the amount of max players
        playerList = GameObject.FindGameObjectsWithTag("Player");
        if (playerList.Length == maxPlayers && !isGameStarted)
        {
            isGameStarted = true;
            InitialiseGame();
        }

        // Resetting the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            InitialiseGame();
            isGameStarted = true;
            GameObject[] foodToDestroy = GameObject.FindGameObjectsWithTag("Food");
            StartCoroutine(GameCountdown());

            for (int i = 0; i < foodToDestroy.Length; i++)
            {
                Destroy(foodToDestroy[i]);
            }
        }

        // Timer related things
        if (isCounting)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0.0f)
            {
                currentTime = 0.0f;
                isCounting = false;
            }

            UpdateTimerDisplay();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            Gizmos.DrawSphere(respawnPoints[i].position, 1);
        }
    }

    void InitialiseGame()
    {
        //Debug.Log("game started");

        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].transform.position = respawnPoints[i].position;
            playerList[i].GetComponent<PlayerController>().canMove = false;
        }

        StartCoroutine(GameCountdown());
    }

    private void UpdateTimerDisplay()
    {
        // Format the time as minutes and seconds (e.g., "01:30")
        string minutes = Mathf.Floor(currentTime / 60).ToString("00");
        string seconds = (currentTime % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;
    }

    void EndGame()
    {

    }

    IEnumerator GameCountdown()
    {

        for (int i = 3; i > 0; i--)
        {
            //Debug.Log("Countdown: " + i);
            titleText.text = (i.ToString());
            yield return new WaitForSeconds(1f);

            if (i == 1)
            {
                titleText.text = ("FLING!");
                yield return new WaitForSeconds(1f);
                titleText.text = null;
                isCounting = true;

                for (int e = 0; e < playerList.Length; e++)
                {
                    playerList[e].GetComponent<PlayerController>().canMove = true;
                }
            }
        }
    }
}
