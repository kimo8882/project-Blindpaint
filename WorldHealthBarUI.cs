using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("Missing PlayerHealth reference.");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogError("Missing Slider reference.");
            return;
        }

        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;

        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}