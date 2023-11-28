using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsValues : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI positionText;
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] Image playerBackground;

    public string playerName;
    public string position;
    public int killCount;
    public Color playerColour;


    public void Start()
    {
        playerNameText.text = playerName;
        playerNameText.color = playerColour;
        playerBackground.color = playerColour;
        killCountText.text = killCount.ToString();
        positionText.text = position;
    }
}
