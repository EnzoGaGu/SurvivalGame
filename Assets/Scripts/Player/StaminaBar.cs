using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaFill; 
    private PlayerStats playerStats; // Reference to player stats

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();

        if(playerStats == null)
        {
            UnityEngine.Debug.LogError("PlayerStats not found in the scene.");
        }
        else
        {
            playerStats.OnStaminaChange += UpdateStaminaBar;
        }
    }

    // Update is called once per frame
    void UpdateStaminaBar(float stamina, float maxStamina)
    {
        staminaFill.fillAmount = stamina / maxStamina;
    }
}
