using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Skill
{
    public string name;         // Name of the skill
    public int tier;            // Skill tier
    public int basePower;       // Base power of the skill (e.g., 15 for healing or damage)
    public int cooldownTurns;   // Cooldown duration in turns
    public int currentCooldown; // Remaining turns on cooldown
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

    public void UseSkill()
    {
        currentCooldown = cooldownTurns;
    }

    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}

