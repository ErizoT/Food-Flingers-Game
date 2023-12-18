using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 2;
    [SerializeField] float invulnerabilityTime = 2f;
    public int kills;
    public ParticleSystem deathEffect;
    public RespawnManager respawnManager;
    private bool invulnerable;
    
    [SerializeField] SkinnedMeshRenderer rend;
    [HideInInspector] public PlayerController playerController;

    [HideInInspector] public int playerIndex;
    [HideInInspector] public Color playerColor;
    [HideInInspector] public Material playerMat;

    private GameObject manager;

    [Header("HUD Stuff")]
    public GameObject playerHUD;
    [SerializeField] Image backgroundColor;
    [SerializeField] TextMeshPro playerNumber;
    public TextMeshProUGUI healthText;
    [SerializeField] TextMeshPro playerText;
    public TextMeshProUGUI scoreText;

    public void Start()
    {
        // OnStart, look for an object in the scene called "Respawn Manager"
        manager = GameObject.Find("Respawn Manager");
        respawnManager = manager.GetComponent<RespawnManager>();

        playerController = GetComponent<PlayerController>();

        // ============= Configure HUDS =============
        backgroundColor.color = playerColor;
        playerNumber.color = playerColor;
        int p = playerIndex + 1;
        playerText.SetText("P" + p.ToString());

        switch (playerIndex)
        {
            case 0:
                playerHUD.GetComponent<RectTransform>().anchoredPosition = new Vector2(-847, 428);
                break;

            case 1:
                playerHUD.GetComponent<RectTransform>().anchoredPosition = new Vector2(847, 428);
                break;

            case 2:
                playerHUD.GetComponent<RectTransform>().anchoredPosition = new Vector2(-847, -372);
                break;

            case 3:
                playerHUD.GetComponent<RectTransform>().anchoredPosition = new Vector2(847, -372);
                break;
        }
    }

    private void Update()
    {   
        playerText.transform.LookAt(Camera.main.transform);
        playerText.transform.Rotate(Vector3.up, 180.0f);

        healthText.text = playerHealth.ToString();
        scoreText.text = kills.ToString(); // I hate this, i really dont want to calculate kills every frame, but i cbs changing it cus of deadline

        deathEffect.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));
    }

    public void OnHit()
    {
        playerHealth -= 1;
        healthText.text = playerHealth.ToString();

        if (playerHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            StartCoroutine(InvulnerabilityWindow());
            deathEffect.Stop(); // Stop emitting particles
            deathEffect.Clear(); // Clear existing particles
            deathEffect.Play();
        }
    }

    public void OnDeath()
    {
        deathEffect.Play();

        playerController.canMove = false;
        playerController.playerSpeed = 0f;
        playerController.Drop();

        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning");

        // Change to trasnparent material
        //MeshRenderer rend = GetComponent<MeshRenderer>();
        Material mat = rend.material;
        Color matColor = mat.color;
        matColor.a = 0.5f;
        mat.color = matColor;
        healthText.text = playerHealth.ToString();
        invulnerable = true;
        StartCoroutine(InvulnerabilityFlash());

        // Wait 3 seconds before respawning
        yield return new WaitForSeconds(3f);

        invulnerable = false;

        gameObject.layer = LayerMask.NameToLayer("Default"); // Reset the player's layer

        // Re-enable movement
        GetComponent<PlayerController>().canMove = true;
        GetComponent<PlayerController>().playerSpeed = 50f;

        // Respawn at a random spawn point
        int randomIndex = Random.Range(0, respawnManager.respawnPoints.Length);
        transform.position = respawnManager.respawnPoints[randomIndex].position;

        // Reset Health
        playerHealth = 2;

        // Reset material transparency
        matColor.a = 1f;
        mat.color = matColor;

        // Resets animation
        GetComponent<PlayerInputHandler>().Respawn();

        StartCoroutine(InvulnerabilityWindow());
    }

    IEnumerator InvulnerabilityWindow()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        invulnerable = true;
        StartCoroutine(InvulnerabilityFlash());
        yield return new WaitForSeconds(invulnerabilityTime);

        invulnerable = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    IEnumerator InvulnerabilityFlash()
    {
        Debug.Log("Should be invulnerable");
        
        rend.enabled = false;
        yield return new WaitForSeconds(0.1f);
        rend.enabled = true;
        yield return new WaitForSeconds(0.1f);

        if(invulnerable)
        {
            StartCoroutine(InvulnerabilityFlash());
        }
    }
}
