using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int playerKills;
    public TextMeshPro scoreDisplay;
    public Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        scoreDisplay.text = playerKills.ToString();

        scoreDisplay.transform.LookAt(mainCamera.transform);
    }
}
