using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // Base skill templates
    private List<string> skillNames = new List<string> { "Heal", "Pound", "Haste", "Intimidate" };

    public Skill GenerateSkill(string skillType, int tier)
    {
        int basePower = skillType == "Heal" || skillType == "Pound" ? 15 : 0;
        int cooldown = skillType == "Haste" ? 5 : 3; // Customize cooldowns
        return new Skill(skillType, tier, basePower, cooldown, skillType);
    }


    // Get a random skill for a given level
    public Skill GetSkillByLevel(int level)
    {
        int tier = Mathf.FloorToInt(level / 5f) + 1; // Tier increases every 5 levels
        string randomSkillName = skillNames[Random.Range(0, skillNames.Count)];

        return GenerateSkill(randomSkillName, tier);
    }
}
