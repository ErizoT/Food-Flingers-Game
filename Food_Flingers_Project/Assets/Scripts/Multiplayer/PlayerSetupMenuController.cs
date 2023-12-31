using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int PlayerIndex;

    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private GameObject readyPanel;

    [SerializeField] private GameObject menuPanel;

    [SerializeField] private Button readyButton;

    private float ignoreInputTime = .5f;
    private bool inputEnabled;

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        titleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    private void Update()
    {
        if(Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetMaterial(Material mat)
    {
        if(!inputEnabled) 
        {
            return;
        }
        PlayerConfigurationManager.Instance.SetPlayerColor(PlayerIndex, mat);
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    public void SetColor(Color color)
    {
        if (!inputEnabled)
        {
            return;
        }
        PlayerConfigurationManager.Instance.SetPlayerBackground(PlayerIndex, color);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
