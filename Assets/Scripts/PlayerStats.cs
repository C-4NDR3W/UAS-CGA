using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // Singleton Instance

    // Stats Pacman
    public int health = 100;
    public int attackPower = 10;
    public int defense = 5;
    public int experience = 0;
    public int coins = 0;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Tetap ada di semua scene
     
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        health -= damageTaken;
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void GainExperience(int exp)
    {
        experience += exp;
        // Tambahkan logika untuk level up jika diperlukan
    }
}   
