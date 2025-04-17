using System;
using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // Singleton

    public event Action<int> OnHotbarSizeChange;
    public event Action<float> OnMaxHealthChange;
    public event Action<float, float> OnHealthChange;
    public event Action<float, float> OnStaminaChange;

    public string playerName = "Player"; // Nombre del jugador

    public float maxHealth = 100;
    public float health;

    public float maxStamina = 100;
    public float stamina = 100;

    public int inventorySize = 30;
    public int inventoryRowSize = 10; 

    public int maxHotbarSize = 10;
    [SerializeField]
    public int _hotbarSize = 10;
    public int hotbarSize
    {
        get => _hotbarSize;
        set
        {
            if (_hotbarSize != value)
            {
                _hotbarSize = value;
                OnHotbarSizeChange?.Invoke(_hotbarSize);
            }
        }
    }


    private bool isInvulnerable = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        health = maxHealth;
        OnHealthChange?.Invoke(health, maxHealth);
        OnStaminaChange?.Invoke(stamina, maxStamina);
    }

    private void OnValidate()
    {
        OnHotbarSizeChange?.Invoke(_hotbarSize);
        OnHealthChange?.Invoke(health, maxHealth);
        OnStaminaChange?.Invoke(stamina, maxStamina);
    }

    public void IncreaseHealth(int amount)
    {
        if (health+amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);

            OnHealthChange?.Invoke(health, maxHealth); // Notifica el cambio de vida

            if (health <= 0)
            {
                //Die();
                UnityEngine.Debug.Log("Player died");
            }

            StartCoroutine(InvulnerabilityTime());
        }
    }

    public void IncreaseStamina(float amount)
    {
        if (stamina + amount > maxStamina)
        {
            stamina = maxStamina;
        }
        else
        {
            stamina += amount;
        }

        OnStaminaChange?.Invoke(stamina, maxStamina); // Notifica el cambio de stamina
    }

    public void DecreaseStamina(float amount)
    {
        stamina -= amount;
        if (stamina <= 0)
        {
            stamina = 0;
        }

        OnStaminaChange?.Invoke(stamina, maxStamina); // Notifica el cambio de stamina
    }

    public void ChangeHotbarSize(int newSize)
    {
        if(hotbarSize != newSize && newSize < maxHotbarSize)
        {
            hotbarSize = newSize;
            OnHotbarSizeChange?.Invoke(hotbarSize);
        }
    }

    private IEnumerator InvulnerabilityTime()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(0.5f);
        isInvulnerable = false;
    }
}
