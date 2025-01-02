using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : BaseEnemyStats
{
    public void Initialize(int playerLevel)
    {
        level = Mathf.Max(1, playerLevel + Random.Range(-2, 1));
        maxHp = 15 + (10 * level) + Random.Range(0, 5);
        currentHp = maxHp;
        attackPower = 5 + (2 * level) + Random.Range(0, 5);
        defenseModifier = 1 + (1 * playerLevel);
        xpReward = 15 + (Random.Range(0, playerLevel) * 3);
    }
}
