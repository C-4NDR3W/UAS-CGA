using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    public int bossLevel;
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public int defenseModifier;
    public int xpReward;

    public void Initialize(int playerLevel)
    {
        bossLevel = Mathf.Max(1, playerLevel + Random.Range(0, 3));
        maxHp = 25 + (7 * bossLevel) + Random.Range(5, 10);
        currentHp = maxHp;
        attackPower = 20 + (2 * bossLevel) + Random.Range(2, 5);
        defenseModifier = 1 + Mathf.RoundToInt(1.5f * playerLevel);
        xpReward = 100 + (Random.Range(100, playerLevel) * 3);
    }

    public int getXP()
    {
        return xpReward;
    }

    public void TakeDamage(int damage)
    {
        int defenseValue = defenseModifier + Random.Range(0, 1 * bossLevel);
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

    public int HealAmount()// this is for sending inverse amount, to send back to damage
    {
        return Random.Range(maxHp / 3, maxHp / 2);
    }

    public void HealEnemy()
    {
        currentHp += Random.Range(maxHp / 3, maxHp / 2);
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

    public int SpecialAttack(){
        int takenDamage = attackPower * 2;
        return takenDamage;
    }

}
