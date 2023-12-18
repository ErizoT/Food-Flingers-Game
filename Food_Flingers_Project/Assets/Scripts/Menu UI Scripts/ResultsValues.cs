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
    [SerializeField] SkinnedMeshRenderer playerModel;
    [SerializeField] Camera renderCamera;
    [SerializeField] RenderTexture[] renderTextureArray;
    [SerializeField] RawImage image;

    [SerializeField] Animator openAnim;

    public Animator raccoonAnim;
    public string playerName;
    public string position;
    public int killCount;
    public Color playerColour;
    public Material playerMaterial;
    public int playerIndex;

    public void Start()
    {
        playerNameText.text = playerName;
        playerNameText.color = playerColour;
        playerBackground.color = playerColour;
        killCountText.text = killCount.ToString();
        positionText.text = position;
        playerModel.material = playerMaterial;
        //openAnim.Play("resultsAnim");

        renderCamera.targetTexture = renderTextureArray[playerIndex];
        image.texture = renderTextureArray[playerIndex];

        GameObject.Find("Game Canvas").SetActive(false);
    }
}
