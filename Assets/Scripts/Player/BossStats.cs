using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : BaseEnemyStats
{
    public void Initialize(int playerLevel)
    {
        level = Mathf.Max(1, playerLevel + Random.Range(0, 3));
        maxHp = 50 + (5 * level) + Random.Range(1, 10);
        currentHp = maxHp;
        attackPower = 20 + Mathf.RoundToInt(1.1f * level) + Random.Range(5, 10);
        defenseModifier = 1 + Mathf.RoundToInt(1.5f * playerLevel);
        xpReward = 100 + (Random.Range(100, playerLevel) * 5);
    }
    public override int HealAmount()
    {
        return Random.Range(maxHp / 3, maxHp / 2);
    }

    public int ReversalAttack()
    {
        return attackPower * 2;
    }

    public void Phase2()
    {
        level++;
        maxHp = maxHp + (level * 5);
        currentHp = maxHp;
        attackPower = attackPower + level;
    }
}
