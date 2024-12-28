using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Skill
{
    public string name;
    public int tier;
    public int basePower;
    public int cooldownTurns;
    public int currentCooldown;
    public string effect;

    public Skill(string name, int tier, int basePower, int cooldownTurns, string effect)
    {
        this.name = name;
        this.tier = tier;
        this.basePower = basePower;
        this.cooldownTurns = cooldownTurns;
        this.currentCooldown = 0;
        this.effect = effect;
    }

    public void UseSkill(PlayerStats player, EnemyStats target = null)
    {
        if (currentCooldown > 0)
        {
            Debug.Log($"Skill {name} is on cooldown for {currentCooldown} more turns!");
            return;
        }

        switch (name)
        {
            case "Heal":
                player.currentHealth = Mathf.Min(player.currentHealth + basePower, player.maxHealth);
                player.healthBar.SetHealth(player.currentHealth);
                Debug.Log($"Player healed for {basePower} HP (Tier {tier}).");
                break;

            case "Pound":
                if (target != null)
                {
                    target.TakeDamage(basePower);
                    Debug.Log($"Dealt {basePower} damage to the enemy (Tier {tier}).");
                }
                break;

            case "Haste":
                Debug.Log($"Player is hastened (Tier {tier}). Implement haste effects here.");
                break;

            case "Intimidate":
                Debug.Log($"Enemy attack power reduced temporarily (Tier {tier}). Implement debuff effects.");
                break;

            default:
                Debug.LogError("Unknown skill used.");
                break;
        }

        currentCooldown = cooldownTurns; // Start the cooldown
    }

    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
