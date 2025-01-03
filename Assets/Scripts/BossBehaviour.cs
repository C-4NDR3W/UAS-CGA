using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour //this class is a modified version of GhostBehaviour. Probably shouldve used some inheritance.
{
    public Vector3 originalPosition;
    public GameObject treasurePrefab;

    private Animator pacmanAnimator;
    private PlayerMovement playerMovement;
    private bool isBattle = false;
    private GameObject battleUIPanel; // Assign this in the Inspector or dynamically
    private bool canMove = true; // Assuming this controls the movement state

    private void Start()
    {
        if (battleUIPanel == null)
        {
            battleUIPanel = InGameUI.Instance.battleUIPanel;
        }

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman") && !isBattle)
        {
            isBattle = true;

            pacmanAnimator = other.GetComponent<Animator>();
            playerMovement = other.GetComponent<PlayerMovement>();

            BossBattleController battleController = FindObjectOfType<BossBattleController>();
            if (battleController != null)
            {
                Vector3 originalPosition = other.transform.position;
                Quaternion originalRotation = other.transform.rotation;

                playerMovement.enabled = false; // Disable movement
                pacmanAnimator.SetBool("isWalking", false); // Stop walking animation
                BossBehaviour bossBehaviour = gameObject.GetComponent<BossBehaviour>();
                if (bossBehaviour != null)
                {
                    bossBehaviour.SetMovement(false);
                }

                // Set up battle state in the BattleController
                battleController.SetPlayerState(originalPosition, originalRotation, playerMovement);
                battleUIPanel.SetActive(true);
                battleController.SetupBattle(this);

                // Relocate Pacman and boss to battle positions
                RelocateForBattle(other.gameObject);
            }
        }
    }

    private void RelocateForBattle(GameObject pacman)
    {
        pacman.transform.position = new Vector3(15.322f, 27.94f, 0.49f);
        pacman.transform.rotation = Quaternion.Euler(0f, 135f, 0f);

        transform.position = new Vector3(18.8f, 28.48f, 1.85f);
        transform.rotation = Quaternion.Euler(-90f, -45f, 0f);
    }

    public void OnBattleEnd()
    {
        // Stop any movement and cleanup logic for the boss
        canMove = false;
        isBattle = false;

        Destroy(gameObject);
    }

    public void SetMovement(bool enable)
    {
        canMove = enable;
    }

    public void SpawnTreasure()
    {
        if (treasurePrefab != null)
        {
            Instantiate(treasurePrefab, transform.position, Quaternion.identity);
        }
    }

    public void OpenStairs()
    {
        Transform entrances = transform.Find("Entrances");
        if (entrances == null)
        {
            Debug.LogError("Entrances GameObject not found!");
            return;
        }

        foreach (Transform child in entrances)
        {
            Destroy(child.gameObject); // Or child.gameObject.SetActive(false)
        }

        Debug.Log("All stairs opened!");
    }
}