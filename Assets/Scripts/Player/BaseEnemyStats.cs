using System.Collections;
using UnityEngine;

public class BaseEnemyStats : MonoBehaviour
{
    public int level;
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public int defenseModifier;
    public int xpReward;

    public int getXP()
    {
        return xpReward;
    }

    public virtual void TakeDamage(int damage)
    {
        int defenseValue = defenseModifier + Random.Range(0, 1 * level);
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

    public virtual int HealAmount()
    {
        return Random.Range(maxHp / 4, maxHp / 2);
    }

    public void HealEnemy()
    {
        currentHp += HealAmount();
        Debug.Log($"Healing target: {this.name}, Healing amount: {HealAmount()}");
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    public virtual int GuardBreak(bool isGuarding)
    {
        return isGuarding ? 2 * attackPower : attackPower / 2;
    }
}
