using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RespawnManager : MonoBehaviour
{
    public GameObject[] playerList;
    public int maxPlayers = 2;
    public Transform[] respawnPoints;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI winnerText;
    [SerializeField] GameObject playerPrefab;

    public bool isGameStarted;

    // Game Timers
    public float totalTime = 60f; // Total time for the countdown in seconds
    private float currentTime;
    private bool isCounting = false;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Music")]
    [SerializeField] AudioSource levelTheme;
    [SerializeField] AudioSource resultsTheme;
    [SerializeField] AudioSource countdownSound;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();

        // Initialise player Configurations
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();

        for(int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, respawnPoints[i].position, respawnPoints[i].rotation, gameObject.transform);
            player.GetComponent<PlayerInputHandler>().InitialisePlayer(playerConfigs[i]);
            player.GetComponent<PlayerHealth>().playerIndex = playerConfigs[i].PlayerIndex;
            player.GetComponent<PlayerHealth>().playerColor = playerConfigs[i].PlayerColor;
        }
    }

    public void Update()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        /*// Initialising player list and starting when players match the amount of max players
        if (playerList.Length == maxPlayers && !isGameStarted)
        {
            //isGameStarted = true;
            InitialiseGame();
        }*/



        // Resetting the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            InitialiseGame();
            isGameStarted = true;
            GameObject[] foodToDestroy = GameObject.FindGameObjectsWithTag("Food");
            currentTime = totalTime;
            isCounting = false;
            StartCoroutine(GameCountdown());

            for (int i = 0; i < foodToDestroy.Length; i++)
            {
                foodToDestroy[i].GetComponent<ProjectileBehaviour>().DefaultDestroy();
            }

            for (int i = 0; i < playerList.Length; i++)
            {
                playerList[i].GetComponent<PlayerHealth>().kills = 0;
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
                StartCoroutine(EndGame());
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

    public void InitialiseGame()
    {
        Debug.Log("game started");
        isGameStarted = true;
        winnerText.text = "";
        Time.timeScale = 1;
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].transform.position = respawnPoints[i].position;
            playerList[i].GetComponent<PlayerController>().canMove = false;
            playerList[i].GetComponent<PlayerController>().playerSpeed = 0f;
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

    IEnumerator EndGame()
    {
        titleText.text = "TIME!";
        Time.timeScale = 0.5f;
        //isGameStarted = false;

        yield return new WaitForSeconds(2f);

        titleText.text = "Game Over!";
        GameObject topPlayer = null;
        GameObject secondTopPlayer = null;
        int highestKills = 0;
        int secondHighestKills = 0;
        resultsTheme.Play();

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].GetComponent<PlayerHealth>().kills > highestKills)
            {
                secondTopPlayer = topPlayer;
                secondHighestKills = highestKills;
                
                highestKills = playerList[i].GetComponent<PlayerHealth>().kills;
                topPlayer = playerList[i];
                int playerNumber = i + 1;
                winnerText.text = "Player " + playerNumber.ToString() + " wins!"; // Corrected ToString() call
            }
            else if ( secondHighestKills == highestKills)
            {
                Debug.Log("Draw");
            }
        }

        yield return new WaitForSeconds(3f);
        Time.timeScale = 1f;

        // Find the object by name
        GameObject playerConfigManager = GameObject.Find("PlayerConfigurationManager");

        // Check if the object was found
        if (playerConfigManager != null)
        {
            SceneTransition.inGame = false;
            Destroy(playerConfigManager);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Object with the specified name was not found
            Debug.LogWarning("PlayerConfigurationManager not found.");
        }
    }

    IEnumerator GameCountdown()
    {

        for (int i = 3; i > 0; i--)
        {
            //Debug.Log("Countdown: " + i);
            titleText.text = (i.ToString());
            countdownSound.Play();
            yield return new WaitForSeconds(1f);
            countdownSound.Stop();

            if (i == 1)
            {
                levelTheme.Play();
                countdownSound.Play();
                titleText.text = ("FLING!");
                yield return new WaitForSeconds(1f);
                titleText.text = null;
                isCounting = true;

                for (int e = 0; e < playerList.Length; e++)
                {
                    playerList[e].GetComponent<PlayerController>().canMove = true;
                    playerList[e].GetComponent<PlayerController>().playerSpeed = 11f; // This is hardcoded and I don't like it
                }
            }
        }
    }
}
