using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    // Stats Pacman
    public int maxHealth = 100;
    public int currentHealth;
    public int attackPower = 10;
    public int coins = 0;
    public int level = 1;
    public int xpPoints;
    public int xpToNextLevel;

    public HealthBar healthBar;
    public TMP_Text coinText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Tetap ada di semua scene

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        UpdateCoinUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void Heal(int cost)
    {
        coins -= cost;
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        UpdateCoinUI();
    }

    public void AddCoins(float multiplier = 1.0f)
    {
        int amount = Random.Range(1, 11);
        int totalReward = Mathf.CeilToInt(amount * multiplier); 
        coins += totalReward;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }

    public void AddExperience(int amount)
    {
        xpPoints += amount;

        if (xpPoints >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        xpPoints -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f); // Scale XP needed
        maxHealth += 20; // Increase HP on level up
        attackPower += 5; // Increase attack power
        currentHealth = maxHealth; // Fully heal the player
    }
}
