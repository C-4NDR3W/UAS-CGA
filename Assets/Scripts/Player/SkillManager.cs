using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private List<string> skillNames = new List<string> { "Heal", "Pound", "Haste", "Intimidate", "Inverse" };

    // Generate a skill based on type and tier
    public Skill GenerateSkill(string skillType, int tier)
    {
        int basePower = skillType == "Heal" || skillType == "Pound" ? 20 * tier : 0;
        int cooldown = 3;
        return new Skill(skillType, tier, basePower, cooldown, skillType);
    }

    // Get a random skill name from the available pool
    public string GetRandomSkillName()
    {
        return skillNames[Random.Range(0, skillNames.Count)];
    }

    // Helper for generating a skill for a given level
    public Skill GetSkillByLevel(int level)
    {
        int tier = Mathf.FloorToInt(level / 5f) + 1;
        string randomSkillName = GetRandomSkillName();
        return GenerateSkill(randomSkillName, tier);
    }
}

