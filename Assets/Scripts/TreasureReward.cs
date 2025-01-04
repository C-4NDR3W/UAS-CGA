using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TreasureReward : MonoBehaviour
{
    public GameObject dialogBox;
    public GameObject doctorUIPanel;
    public TMP_Text dialogText;
    public GameObject ghostPrefab;

    void Start()
    {
        doctorUIPanel = InGameUI.Instance.doctorUIPanel;
        if (doctorUIPanel != null)
        {
            dialogBox = doctorUIPanel.transform.Find("TextBox")?.gameObject;

            // Check if both dialogBox and buttons are found
            if (dialogBox != null)
            {
                // Set them inactive initially
                dialogBox.SetActive(false);
                // Find child elements inside the panels (for example, text and buttons)
                dialogText = dialogBox.transform.Find("Doctor Text")?.GetComponent<TMP_Text>();
            }
            else
            {
                Debug.LogError("Textbox or Buttons panels not found under doctorUIPanel.");
            }
        }
        else
        {
            Debug.LogError("doctorUIPanel is null. Ensure InGameUI.Instance is properly initialized.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pacman"))
        {
            ShowDialog();
        }
    }

    void ShowDialog()
    {
        dialogText.text = "You Found a treasure Chest!";
        dialogBox.SetActive(true);
        TreasureRewardProcess();
    }

    void TreasureRewardProcess()
    {
        int missingHp = PlayerStats.Instance.maxHealth - PlayerStats.Instance.currentHealth;
        float healChance = 0.25f; //0.25
        float coinChance = 0.25f; //0.5
        float mimicChance = 0.25f; //a sudden enemy appears! //0.75
        float xpChance = 0.15f; //0.90
        float permanentAtkBuffChance = 0.05f;
        // float permanentHpBuffChance = 0.05f; //1.0 unused because it is the remaining 0.05 


        float actionRoll = Random.value;

        if (actionRoll < healChance)
        {
            // Heal outcome
            PlayerStats.Instance.Heal(0);
            Debug.Log("Healed for " + missingHp + " HP");
            dialogText.text = "You got healed!";
        }
        else if (actionRoll < healChance + coinChance)
        {
            // Coins outcome
            int randomMult = Random.Range(1, 10);
            PlayerStats.Instance.AddCoins(randomMult);
            Debug.Log($"You got {randomMult} coins!");
            dialogText.text = "You got Coins!";
        }
        else if (actionRoll < healChance + coinChance + mimicChance)
        {
            if (ghostPrefab != null)
            {
                Instantiate(ghostPrefab, transform.position, Quaternion.identity);
                Debug.Log("A Mimic (Ghost) has spawned at the treasure chest's location!");
                dialogText.text = "A Mimic Appears!";
            }
            else
            {
                Debug.LogError("Ghost prefab (Mimic) is not assigned in the Inspector!");
            }
        }
        else if (actionRoll < healChance + coinChance + mimicChance + xpChance)
        {
            int xpAmount = PlayerStats.Instance.level * 10;
            PlayerStats.Instance.AddExperience(xpAmount);
            Debug.Log($"You gained {xpAmount} XP!");
            dialogText.text = "You gained XP!";
        }
        else if (actionRoll < healChance + coinChance + mimicChance + xpChance + permanentAtkBuffChance)
        {
            // Permanent attack buff outcome
            PlayerStats.Instance.attackPower += 3; // Assuming permanentAttack exists
            Debug.Log("Your attack power permanently increased by 3!");
            dialogText.text = "Your attack power permanently increased by 3!";
        }
        else
        {
            PlayerStats.Instance.maxHealth += 10; // Assuming permanentMaxHealth exists
            PlayerStats.Instance.healthBar.IncreaseMaxHealth(PlayerStats.Instance.maxHealth);
            Debug.Log("Your max health permanently increased by 10!");
            dialogText.text = "Your max health permanently increased by 10!";
        }

        // Hide the dialog and destroy the chest
        Invoke(nameof(HideDialog), 1f);
        Destroy(gameObject, 1.1f);
    }

    void HideDialog()
    {
        dialogBox.SetActive(false);
    }
}
