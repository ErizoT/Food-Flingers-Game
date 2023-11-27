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

    [SerializeField] Animator anim;

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

        if(!ArrayContainsMaterial(PlayerConfigurationManager.Instance.selectedMats, mat))
        {
            PlayerConfigurationManager.Instance.SetPlayerColor(PlayerIndex, mat);
            PlayerConfigurationManager.Instance.selectedMats.Add(mat);
            readyPanel.SetActive(true);
            readyButton.Select();
            menuPanel.SetActive(false);
        } else
        {
            anim.SetTrigger("error");
        }
    }

    public void SetColor(Color color)
    {
        if (!inputEnabled)
        {
            return;
        }

        if (!ArrayContainsColor(PlayerConfigurationManager.Instance.selectedColours, color))
        {
            PlayerConfigurationManager.Instance.SetPlayerBackground(PlayerIndex, color);
            PlayerConfigurationManager.Instance.selectedColours.Add(color);
        }
        else
        {
            anim.SetTrigger("error");
        }
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
    }

    bool ArrayContainsMaterial(List<Material> array, Material material)
    {
        foreach (Material arrayMaterial in array)
        {
            if (arrayMaterial == material)
            {
                return true; // Material found in the array
            }
        }
        return false; // Material not found in the array
    }

    bool ArrayContainsColor(List<Color> array, Color color)
    {
        foreach (Color arrayColor in array)
        {
            if (arrayColor == color)
            {
                return true; // Material found in the array
            }
        }
        return false; // Material not found in the array
    }
}
