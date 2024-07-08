using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Image foregroundImage; // Assign this in the Unity Editor
    public int regenAmount = 10; // Amount of health to regenerate each tick
    public float regenTickTime = 2f; // Time between each regeneration tick

    private Coroutine regenCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Starting health: " + currentHealth); // Log initial health
        UpdateHealthBar();
        regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Taking damage: {amount}, Current Health: {currentHealth}"); // Log for damage

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        Debug.Log($"Healing: {amount}, Current Health: {currentHealth}"); // Log for healing

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
            Debug.Log($"Updating health bar: {fillAmount}"); // Log for health bar update
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
            Debug.Log("Regenerating health, current health: " + currentHealth); // Debug log for health regeneration
        }
    }
}
