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

    [Header("Results")]
    [SerializeField] GameObject resultsPrefab;

    [Header("Developer Tools")]
    [SerializeField] bool unlimitedTime = false;
    [SerializeField] bool resetButton = false;

    public List<Player> players = new List<Player>();

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();

        if(unlimitedTime)
        {
            totalTime = 999f;
            currentTime = totalTime;
            levelTheme.volume = 0f;
        }

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
        if (Input.GetKeyDown(KeyCode.R) && resetButton)
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

    // Where the game starts. SceneTransition calls InitialiseGame() once every player is ready in the player setup screen
    public void InitialiseGame()
    {
        //Debug.Log("game started");
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
    public class Player
    {
        public GameObject Object;
        public int Kills;
        public string Name;

        public Player(GameObject thing, int kills, string name)
        {
            Object = thing;
            Kills = kills;
            Name = name;
        }
    }

    IEnumerator EndGame()
    {
        titleText.text = "TIME!";
        Time.timeScale = 0.5f;
        //isGameStarted = false;

        yield return new WaitForSeconds(2f);

        /*titleText.text = "Game Over!";
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
        }*/
        // ------------------------------------------------------------

        titleText.text = "";

        var rootMenu = GameObject.Find("Results Canvas");
        resultsTheme.Play();

        for (int i = 0; i < playerList.Length; i++)
        {
            PlayerHealth playerHealth = playerList[i].GetComponent<PlayerHealth>();
            players.Add(new Player(playerList[i], playerHealth.kills, "Player " + (i +1)));
            playerHealth.playerHUD.SetActive(false);
        }

        players.Sort((a, b) => b.Kills.CompareTo(a.Kills));

        for (int i = 0; i < players.Count; i++)
        {
            // Use the sorted 'players' list instead of 'playerList'
            PlayerHealth playerHealth = players[i].Object.GetComponent<PlayerHealth>();
            var menu = Instantiate(resultsPrefab, rootMenu.transform);


            ResultsValues results = menu.GetComponent<ResultsValues>();
            results.playerName = players[i].Name;
            results.playerColour = playerHealth.playerColor;
            results.killCount = playerHealth.kills;
            results.playerMaterial = playerHealth.playerMat;
            results.playerIndex = i;

            if (i == 0)
            {
                results.position = "1st";
                results.raccoonAnim.SetBool("isFirst", true);
            }
            else if (playerHealth.kills == players[0].Object.GetComponent<PlayerHealth>().kills)
            {
                results.position = "1st";
                results.raccoonAnim.SetBool("isFirst", true);
            }
            else if (i == 1)
            {
                results.position = "2nd";
            }
            else if (playerHealth.kills == players[1].Object.GetComponent<PlayerHealth>().kills)
            {
                results.position = "2nd";
            }
            else if (i == 2)
            {
                results.position = "3rd";
            }
            else if (playerHealth.kills == players[2].Object.GetComponent<PlayerHealth>().kills)
            {
                results.position = "3rd";
            }
            else
            {
                results.position = (i + 1).ToString() + "th";
            }

            yield return new WaitForSeconds(0.2f);
        }

        // --------------------------------------------------
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
                    playerList[e].GetComponent<PlayerController>().playerSpeed = 50; // This is hardcoded and I don't like it
                }
            }
        }
    }
}
