using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PacmanDoctor : MonoBehaviour
{
    public GameObject doctorUIPanel;
    public GameObject dialogBox;
    public GameObject buttons;
    public TMP_Text dialogText;     
    public Button yesButton;    
    public Button noButton;
    private int cost = 0;
    private bool isPlayerNearby = false;
    private bool isQuestionDialog = false;

    void Start()
    {
        doctorUIPanel = InGameUI.Instance.doctorUIPanel;
        if (doctorUIPanel != null)
        {
            dialogBox = doctorUIPanel.transform.Find("TextBox")?.gameObject;
            buttons = doctorUIPanel.transform.Find("Buttons")?.gameObject;

            // Check if both dialogBox and buttons are found
            if (dialogBox != null && buttons != null)
            {
                // Set them inactive initially
                dialogBox.SetActive(false);
                buttons.SetActive(false);

                // Find child elements inside the panels (for example, text and buttons)
                dialogText = dialogBox.transform.Find("Doctor Text")?.GetComponent<TMP_Text>();
                yesButton = buttons.transform.Find("Yes Button")?.GetComponent<Button>();
                noButton = buttons.transform.Find("No Button")?.GetComponent<Button>();

                // Add listeners if buttons are found
                if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
                if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
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

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space) && !isQuestionDialog)
        {
            ShowDialog();
        }
        else if (isQuestionDialog && Input.GetKeyDown(KeyCode.Space))
        {
            ShowButtons();
            isQuestionDialog = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pacman"))
        {
            isPlayerNearby = false;
            dialogBox.SetActive(false); 
        }
    }

    void ShowDialog()
    {
        cost = Mathf.RoundToInt((PlayerStats.Instance.maxHealth - PlayerStats.Instance.currentHealth) * 1.25f);

        if (cost > 0)
        {
            dialogText.text = $"Do you want to heal for {cost}?";
            dialogBox.SetActive(true);
            isQuestionDialog = true;
        }
        else
        {
            dialogText.text = "You are already at full health!";
            dialogBox.SetActive(true);
            buttons.SetActive(false);
            Invoke(nameof(HideDialog), 2f);
        }
    }

    void ShowButtons()
    {
        dialogBox.SetActive(false);
        buttons.SetActive(true);
    }

    void OnYesClicked()
    {
        if (PlayerStats.Instance.coins >= cost)
        {
            dialogText.text = $"You are healed for {cost} coins!";
            dialogBox.SetActive(true);
            PlayerStats.Instance.Heal(cost);
            buttons.SetActive(false);
            Invoke(nameof(HideDialog), 2f);
        }
        else
        {
            buttons.SetActive(false);
            dialogText.text = "You don't have enough coins!";
            dialogBox.SetActive(true);
            Invoke(nameof(HideDialog), 2f);
        }
    }

    void OnNoClicked()
    {
        buttons.SetActive(false);
        HideDialog();
    }

    void HideDialog()
    {
        dialogBox.SetActive(false);
    }
}
