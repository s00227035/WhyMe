using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Image foregroundImage; // Assign this in the Unity Editor
    public int regenAmount = 10; // Amount of health to regenerate each tick
    public float regenTickTime = 3f; // Time between each regeneration tick

    private Coroutine regenCoroutine;

    [SerializeField]
    private GameObject playerDeathCanvas;

    private Player player; //Reference to the plaer
    private bool isDead = false; //Check if the player is dead

    //AUDIO
    private AudioSource audioSource;
    private AudioSource damageAudioSource; //Second audio for not intefering with walking
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip damageSound; //Player takes damage sound

    void Start()
    {
        currentHealth = maxHealth;
        //Debug.Log("Starting health: " + currentHealth); // Log initial health
        UpdateHealthBar();
        regenCoroutine = StartCoroutine(RegenerateHealth());

        //Find Player component
        player = GetComponent<Player>();

        //Ensure PlayerDeathCanvas is assigned
        if (playerDeathCanvas == null)
        {
            Debug.LogError("PlayerDeathCanvas is not assigned in the inspector");
        }
        else
        {
            playerDeathCanvas.SetActive(false); //Ensure it's inactive at start
        }

        //AudioSource component
        audioSource = GetComponent<AudioSource>();

        damageAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return; //No damage if player is dead
        }

        currentHealth -= amount;
        //Debug.Log($"Taking damage: {amount}, Current Health: {currentHealth}"); // Log for damage

        //Delay the damage sound by 1.3 seconds
        StartCoroutine(PlayDelayedDamageSound(1.3f));

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }

        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        if (isDead)
        {
            return; //No healing if player is dead
        }

        currentHealth += amount;
        //Debug.Log($"Healing: {amount}, Current Health: {currentHealth}"); // Log for healing

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (foregroundImage != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            //Debug.Log($"Updating health bar: {fillAmount}"); // Log for health bar update
            foregroundImage.fillAmount = fillAmount;
        }
        else
        {
            Debug.LogError("Foreground image is not assigned.");
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                Heal(regenAmount);
            }
            yield return new WaitForSeconds(regenTickTime);
            //Debug.Log("Regenerating health, current health: " + currentHealth); // Debug log for health regeneration
        }
    }

    //Player's death
    private void PlayerDeath()
    {
        //Debug.Log("Player died");

        isDead = true;

        //Disable player input
        if (player != null)
        {
            player.SetInputEnabled(false);
        }

        //Show death UI
        if (playerDeathCanvas != null)
        {
            playerDeathCanvas.SetActive(true);
        }

        //Stop health regen
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        //Stop time
        Time.timeScale = 0f;
    }

    //Restart the game
    public void RestartGame()
    {
        PlayClickSound(); //Click sound
        Time.timeScale = 1f; //Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Return to MainMenu
    public void GoToMainMenu()
    {
        PlayClickSound(); //Click sound
        Time.timeScale = 1f; //Resume time
        SceneManager.LoadScene("MainMenu"); //Load main menu
    }

    //Hover method
    public void OnButtonHover()
    {
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound); //Play hover sound
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); //Play click sound
        }
    }

    private IEnumerator PlayDelayedDamageSound(float delay)
    {
        yield return new WaitForSeconds(delay); //Wait for the daley

        if (damageSound != null && damageAudioSource != null)
        {
            damageAudioSource.PlayOneShot(damageSound); //Play damage sound
        }
    }
}
