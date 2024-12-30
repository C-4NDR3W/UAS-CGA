using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    // Stats Pacman
    public int maxHealth = 100;
    public int currentHealth;
    public int attackPower = 15;
    public int coins = 0;
    public int level = 1;
    public int xpPoints = 0;
    public int xpToNextLevel = 25;
    public List<Skill> skills = new List<Skill>();
    private int maxSkills = 3;

    public HealthBar healthBar;
    public TMP_Text coinText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Tetap ada di semua scene
        InitializeUI();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        UpdateCoinUI();

        InitializeStartingSkill();
    }

    private void InitializeStartingSkill()
    {
        SkillManager skillManager = FindObjectOfType<SkillManager>();
        Skill startingSkill = skillManager.GenerateSkill(skillManager.GetRandomSkillName(), 1);
        skills.Add(startingSkill);
        Debug.Log($"Starting skill added: {startingSkill.name} (Tier {startingSkill.tier})");
    }

    public void RewardSkillAfterBattle()
    {
        SkillManager skillManager = FindObjectOfType<SkillManager>();
        int currentTier = Mathf.FloorToInt(level / 5f) + 1;

        // Generate a random skill from the available options
        Skill newSkill = skillManager.GenerateSkill(skillManager.GetRandomSkillName(), currentTier);
        Debug.Log($"Reward skill: {newSkill.name} (Tier {newSkill.tier})");

        // If the player has less than 3 skills, add the new skill
        if (skills.Count < maxSkills)
        {
            skills.Add(newSkill);
            Debug.Log($"Skill added: {newSkill.name} (Tier {newSkill.tier})");
        }
        else
        {
            // Player already has 3 skills, replace one
            ForgetAndReplaceSkill(newSkill);
        }
    }

    private void ForgetAndReplaceSkill(Skill newSkill) //TODO
    {
        Debug.Log("Player has 3 skills. Replacing a skill...");

        // Present a choice to the player (you can implement a UI popup for selection).
        // For now, we replace a random skill.
        int skillToReplaceIndex = Random.Range(0, skills.Count);
        Debug.Log($"Replacing skill: {skills[skillToReplaceIndex].name} with {newSkill.name}");

        skills[skillToReplaceIndex] = newSkill;
    }

    public void InitializeUI()
    {
        GameObject normalUI = GameObject.Find("In Game UI/NormalUI");

        if (normalUI != null)
        {
            // Find the HealthBar and CoinText components
            HealthBar foundHealthBar = normalUI.GetComponentInChildren<HealthBar>();
            TMP_Text foundCoinText = normalUI.GetComponentInChildren<TMP_Text>();

            // Assign the found UI elements to PlayerStats
            if (foundHealthBar != null && foundCoinText != null)
            {
                healthBar = foundHealthBar;
                coinText = foundCoinText;
            }
            else
            {
                Debug.LogError("HealthBar or CoinText not found in NormalUI.");
            }
        }
        else
        {
            Debug.LogError("NormalUI not found in InGameUI.");
        }
    }

    public void TakeDamage(int damage, bool guardState)
    {
        if (guardState != true)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth -= damage / 2;
        }

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
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f); // Scale XP needed
        maxHealth += 20; // Increase HP on level up
        attackPower += 5; // Increase attack power
        currentHealth = maxHealth; // Fully heal the player
    }

    
}
