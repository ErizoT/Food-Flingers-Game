using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// What this class does:
// - Persists between Unity scenes
// - Stores the configurations of all players (controls, colours, customisation)
// - Acts as a place for things to retrieve these configurations

public class PlayerConfigurationManager : MonoBehaviour
{
    // A list of class 'PlayerConfiguration'
    // Essentially a variable that contains all player configurations
    private List<PlayerConfiguration> playerConfigs;

    // Self-explanatory...max players the configuration manager can hold
    [SerializeField] private int maxPlayers = 4;

    // THE SINGLETON PATTERN
    // we only want a SINGLE instance of the player configuration manager
    // to exist at a time, and the 'Singleton Pattern' is responsible for doing so.
    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        // This checks if there is no Instance (PlayerConfigManager) in the scene
        if (Instance != null)
        {
            Debug.Log("Trying to create another instance of singleton!");
        }

        // - Creates a PlayerConfigurationManager
        // - Makes it so it doesn't destroy itself when transitioning to a new scene
        // - Initializes our player configurations
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }
    }

    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].playerMaterial = color;
    }

    // READY PLAYER FUNCTION
    // - Function that takes a player's index
    // - Accesses player configs and sets 'isReady' bool to true
    // - Checks if the number of playerConfigs match the number of Max Players
    // - Checks if the 'isReady' bool in all playerConfigs are true
    // - Accesses the scene manager to go to the scene 'SampleScene'
    public void ReadyPlayer(int index)
    {
        playerConfigs[index].isReady = true;

        if(playerConfigs.Count == maxPlayers && playerConfigs.All(p => p.isReady == true))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    // PLAYER JOINED HANDLER
    // Function that handles all playerjoined events and stuff
    // - Takes in a Player Input
    // - Checks if the current player hasn't joined already;
    // - Sets the parent of the receiving player input to this PlayerConfigurationManager
    // - Adds a new player configuration to the playerConfigs list
    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player Joined" + pi.playerIndex);

        if (!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }

}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        playerIndex = pi.playerIndex;
        input = pi;
    }

    // Stores the control type of the player (controller, mouse & keyboard)
    public PlayerInput input { get; set; }

    // An interger that indicates which player it is.
    public int playerIndex { get; set; }

    // A bool to indicate whether a player is ready
    public bool isReady { get; set; }

    // Property that stores the player's colour
    public Material playerMaterial { get; set; }

}
