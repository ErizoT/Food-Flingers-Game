using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 2;
    public int kills;
    public ParticleSystem deathEffect;
    public RespawnManager respawnManager;
    public TextMeshPro healthText;
    public TextMeshProUGUI scoreText;

    private GameObject manager;

    public void Start()
    {
        // OnStart, look for an object in the scene called "Respawn Manager"
        manager = GameObject.Find("Respawn Manager");
        respawnManager = manager.GetComponent<RespawnManager>();

        //================================= Will remove this shit soon, this annoys me ================= 

        // Detect other players in the scene by tag
        GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Check the number of other players
        int numberOfOtherPlayers = otherPlayers.Length;

        // Change color based on the number of other players
        if (numberOfOtherPlayers == 1)
        {
            return;
        }
        else if (numberOfOtherPlayers == 2)
        {
            RectTransform textRectTransform = scoreText.rectTransform;
            textRectTransform.anchoredPosition = new Vector2(-341f, -409f);
        }
        else if (numberOfOtherPlayers == 3)
        {
            RectTransform textRectTransform = scoreText.rectTransform;
            textRectTransform.anchoredPosition = new Vector2(-141f, -409f);
        }
        else if (numberOfOtherPlayers == 4)
        {
            RectTransform textRectTransform = scoreText.rectTransform;
            textRectTransform.anchoredPosition = new Vector2(141f, -409f);
        }
        // ==============================================================================
    }

    private void Update()
    {
        healthText.text = playerHealth.ToString();
        healthText.transform.LookAt(Camera.main.transform);
        healthText.transform.Rotate(Vector3.up, 180.0f);

        scoreText.text = kills.ToString();
    }

    public void OnHit()
    {
        playerHealth -= 1;

        if (playerHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            deathEffect.Stop(); // Stop emitting particles
            deathEffect.Clear(); // Clear existing particles
            deathEffect.Play();
        }
    }

    public void OnDeath()
    {
        deathEffect.Stop(); // Stop emitting particles
        deathEffect.Clear(); // Clear existing particles
        deathEffect.Play();

        GetComponent<PlayerController>().canMove = false;
        GetComponent<PlayerController>().playerSpeed = 0f;

        //Rotate the character so it looks dead
        Vector3 deathRotation = new Vector3(0, 180, 90);
        Quaternion newRotation = Quaternion.Euler(deathRotation);
        transform.rotation = newRotation;

        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning");

        // Wait 3 seconds before respawning
        yield return new WaitForSeconds(3f);

        gameObject.layer = LayerMask.NameToLayer("Default"); // Reset the player's layer

        // Re-enable movement
        GetComponent<PlayerController>().canMove = true;
        GetComponent<PlayerController>().playerSpeed = 11f;

        // Reset rotation
        Vector3 deathRotation = new Vector3(0, 0, 0);
        Quaternion newRotation = Quaternion.Euler(deathRotation);
        transform.rotation = newRotation;

        // Respawn at a random spawn point
        int randomIndex = Random.Range(0, respawnManager.respawnPoints.Length);
        transform.position = respawnManager.respawnPoints[randomIndex].position;

        // Reset Health
        playerHealth = 2;
    }
}
