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
        enemyLevel = Mathf.Max(1, playerLevel) + Random.Range(-2, 1);
        maxHp = 15 + (10 * enemyLevel) + Random.Range(0, 5);
        currentHp = maxHp;
        attackPower = 5 + (2 * enemyLevel) + Random.Range(0, 5);
        defenseModifier = 1 + (1 * playerLevel);
        xpReward = 15 + (Random.Range(0, playerLevel) * 3);
    }

    public int getXP()
    {
        return xpReward;
    }

    public void TakeDamage(int damage)
    {
        int defenseValue = defenseModifier + Random.Range(0, 1 * enemyLevel);
        int takenDamage = Mathf.Max(damage - defenseValue, 1);
        currentHp -= takenDamage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

    }

    public void TakeIntimidateDamage(int damage)
    {
        int takenDamage = Mathf.Max(damage, 1);
        currentHp -= takenDamage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public bool isDead()
    {
        return currentHp <= 0;
    }

    public void HealEnemy()
    {
        currentHp += Random.Range(maxHp / 4, maxHp / 2);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // Clamp between 0 and maxHp
    }

    public int GuardBreak(bool isGuarding)
    {
        if (isGuarding == true)
        {
            int takenDamage = 2 * attackPower;
            return takenDamage;
        }
        else
        {
            int takenDamage = attackPower / 2;
            return takenDamage;
        }
    }


}
