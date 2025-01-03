using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public GameObject normalUIPanel;

    // Stats Pacman
    public int maxHealth = 100;
    public int currentHealth;
    public int attackPower = 15;
    public int coins = 0;
    public int level = 1;
    public int xpPoints = 0;
    public int xpToNextLevel;
    public List<Skill> skills = new List<Skill>();
    // private int maxSkills = 3; //seems unused

    public HealthBar healthBar;
    public TMP_Text coinText;

    public GameObject dialogBox;
    public TMP_Text dialogText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Tetap ada di semua scene
        xpToNextLevel = 50;
        InitializeUI();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        UpdateCoinUI();

        InitializeStartingSkill();
    }

    public void UseSkill(Skill skill, BaseEnemyStats target = null)
    {
        if (skill == null)
        {
            Debug.LogError("Skill is null!");
            return;
        }

        if (skill.currentCooldown > 0)
        {
            Debug.Log($"Skill {skill.name} is still on cooldown for {skill.currentCooldown} turns!");
            return;
        }

        // Use the skill and set its cooldown
        skill.UseSkill(this, target);
    }

    public void ReduceCooldowns()
    {
        foreach (Skill skill in skills)
        {
            skill.ReduceCooldown();
        }
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
        skills.Add(newSkill);

    }


    public void InitializeUI()
    {
        GameObject normalUI = GameObject.Find("In Game UI/NormalUI");

        if (normalUI != null)
        {
            // Find the HealthBar and CoinText components
            HealthBar foundHealthBar = normalUI.GetComponentInChildren<HealthBar>();
            TMP_Text foundCoinText = normalUI.GetComponentInChildren<TMP_Text>();
            dialogBox = normalUI.transform.Find("LevelUp TextBox").gameObject;
            dialogText = dialogBox.GetComponentInChildren<TMP_Text>();

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

    public void InversedHeal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
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

    public int getLevel()
    {
        return level;
    }

    private void LevelUp()
    {
        level++;
        xpPoints -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.25f); // Scale XP needed
        maxHealth += 20; // Increase HP on level up
        attackPower += 10; // Increase attack power
        currentHealth = maxHealth; // Fully heal the player

        StartCoroutine(ShowLevelUpDialog());
    }

    private IEnumerator ShowLevelUpDialog()
    {
        dialogBox.SetActive(true);
        dialogText.SetText("You Leveled Up! Max Health and Attack Increased.");

        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds

        dialogBox.SetActive(false);
    }
}
