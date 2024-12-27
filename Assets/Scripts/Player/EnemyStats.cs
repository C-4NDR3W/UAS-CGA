using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int enemyLevel;
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public int defenseModifier;
    public int xpReward;

    public void Initialize(int playerLevel)
    {
        enemyLevel = Mathf.Max(1, playerLevel) + Random.Range(-2, 3);
        maxHp = 15 + (10 * enemyLevel) + Random.Range(0, 5);
        currentHp = maxHp;
        attackPower = 5 + (2 * enemyLevel) + Random.Range(0, 5);
        defenseModifier = 1 + (1 * playerLevel);
        xpReward = 15 + (Random.Range(0, playerLevel) * 3);
    }

    public void TakeDamage(int damage)
    {
        int takenDamage = Mathf.Max(damage - defenseModifier, 1);
        currentHp -= takenDamage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

    }

    public bool isDead()
    {
        return currentHp <= 0;
    }


}
