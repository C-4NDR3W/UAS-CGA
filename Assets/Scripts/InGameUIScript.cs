using UnityEngine;
using TMPro;
public class InGameUIScript : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text coinsText;

    void Start()
    {
        UpdateHealthUI();
        UpdateCoinsUI();
    }

    void Update()
    {
        UpdateHealthUI();
        UpdateCoinsUI();
    }

    void UpdateHealthUI()
    {
        if (PlayerStats.Instance != null)
        {
            healthText.text = "Health: " + PlayerStats.Instance.health.ToString();
        }
    }

    void UpdateCoinsUI()
    {
        if (PlayerStats.Instance != null)
        {
            coinsText.text = "Coins: " + PlayerStats.Instance.coins.ToString();
        }
    }
}
