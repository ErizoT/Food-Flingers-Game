using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    // Any scene can access this class to get differnet player configs
    // Is going to contain player control, number, and control method of each player

    private List<PlayerConfiguration> playerConfigs;

    [SerializeField] int MaxPlayers = 4;

    public static PlayerConfigurationManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null) // Checks if there is another instance in the scene
        {
            Debug.Log("Trying to create another instance of singleton!");
        }
        else // If there isn't create a configuration manager
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }
    }

    public void SetPlayerColor(int index, Material color)
    {
        playerConfigs[index].PlayerMaterial = color;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;

        if (playerConfigs.All(p => p.IsReady == true))
        {
            //Debug.Log("Load Scene");
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player Joined" + pi.playerIndex);
        pi.transform.SetParent(transform); // Parents the joining player to this PlayerConfigurationManager, so they go along for the ride to the next scene

        if(!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex)) // Checking the player index to check if we haven't already added this player
        {
            playerConfigs.Add(new PlayerConfiguration(pi)); // Creates a new PlayerConfig with the incoming player input
        }
    }

}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; set; }

    public int PlayerIndex { get; set; }

    public bool IsReady { get; set; }

    public Material PlayerMaterial { get; set; }

}
