using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFill; 
    private PlayerStats playerStats; // Reference to player stats

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            UnityEngine.Debug.LogError("PlayerStats not found in the scene.");
        }
        else
        {
            playerStats.OnHealthChange += UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float health, float maxHealth)
    {
        healthFill.fillAmount = health / maxHealth;
    }
}
