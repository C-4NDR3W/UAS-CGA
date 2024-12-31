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
        float healChance = 0.5f;
        // float coinChance = 0.5f; //make space for other features

        float actionRoll = Random.value;

        if (actionRoll < healChance)
        {
            PlayerStats.Instance.currentHealth = Mathf.Min(PlayerStats.Instance.currentHealth + missingHp, PlayerStats.Instance.maxHealth);
            Debug.Log("Healed for " + missingHp + " HP");
            dialogText.text = $"You got healed!";
        }
        else
        {
            int randomMult = Random.Range(1, 10);
            PlayerStats.Instance.AddCoins(randomMult);
            dialogText.text = $"You got Coins!";
        }

        Destroy(gameObject);
        Invoke(nameof(HideDialog), 1f);
    }

    void HideDialog()
    {
        dialogBox.SetActive(false);
    }
}
