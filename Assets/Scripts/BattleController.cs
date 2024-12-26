using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public class BattleController : MonoBehaviour
{
    private Vector3 originalPosition; // To store the original position of Pacman
    private Quaternion originalRotation; // To store the original rotation of Pacman
    private bool isPacmanRelocating = false; // To prevent multiple simultaneous relocations
    private PlayerMovement playerMovement; // Reference to Pacman's movement script (PlayerMovement)
    public bool isBattle = false; // Bool to check if the battle is active
    private Animator pacmanAnimator; // Reference to Pacman's Animator component
    public GameObject battleUIPanel; // Reference to the UI Panel for the battle
    public GameObject treasureChest;
    public BattleState state;
    private EnemyStats enemyStats;


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
        if (other.CompareTag("Pacman"))
        {
            if (gameObject.CompareTag("Ghost"))
            {
                // Ghost collision: 10 damage, 1 coin, no treasure chest
                HandleEnemyCollision(other.gameObject, damage: 2);
            }
            else if (gameObject.CompareTag("Boss"))
            {
                // Boss collision: 50 damage, 100 coins, spawn treasure chest
                //reminder change damage prolly for this
                HandleEnemyCollision(other.gameObject, damage: 50);
                RelocateTreasureChest();
            }
        }
    }

    private IEnumerator TemporarilyRelocatePacman(GameObject pacman)
    {
        isPacmanRelocating = true;
        state = BattleState.START;

        // Store the original position of Pacman
        originalPosition = pacman.transform.position;
        originalRotation = pacman.transform.rotation;

        // Disable Pacman's movement (if PlayerMovement is attached)
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // New position to relocate Pacman
        Vector3 newPosition = new Vector3(15.322f, 27.94f, 4.538f);

        // Move Pacman to the new position
        pacman.transform.position = newPosition;

        // Set Pacman's rotation to (0, 160, 0)
        pacman.transform.rotation = Quaternion.Euler(0f, 135f, 0f);

        Vector3 newGhostPosition = new Vector3(18.25f, 28.17f, 1.85f);
        Quaternion newGhostRotation = Quaternion.Euler(0f, -45f, 0f);
        gameObject.transform.position = newGhostPosition;
        gameObject.transform.rotation = newGhostRotation;
        GhostBehaviour ghostBehaviour = gameObject.GetComponent<GhostBehaviour>();
        if (ghostBehaviour != null)
        {
            ghostBehaviour.SetMovement(false);
        }

        enemyStats = gameObject.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            int playerLevel = PlayerStats.Instance.level; // Get player's level
            enemyStats.Initialize(playerLevel);
        }

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;


        // // Check if the original position is valid and return Pacman to it
        // if (originalposition != vector3.zero)
        // {
        //     pacman.transform.position = originalposition;
        //     pacman.transform.rotation = originalrotation;
        // }

        // // Re-enable Pacman's movement
        // if (playerMovement != null)
        // {
        //     playerMovement.enabled = true;
        // }

        // isBattle = false;
        // battleUIPanel.SetActive(false);
        // isPacmanRelocating = false;
        // Destroy(gameObject);
        // PlayerStats.Instance.AddCoins(1);
    }

    void HandleEnemyCollision(GameObject pacman, int damage)
    {
        if (!isPacmanRelocating)
        {
            isBattle = true;

            // Set Pacman's state
            pacmanAnimator = pacman.GetComponent<Animator>();
            playerMovement = pacman.GetComponent<PlayerMovement>();

            if (battleUIPanel != null && isBattle)
            {
                pacmanAnimator.SetBool("isWalking", false);
                battleUIPanel.SetActive(true); // Show the battle UI panel
            }

            // Apply damage and reward
            PlayerStats.Instance.TakeDamage(damage);

            // Handle temporary relocation
            StartCoroutine(TemporarilyRelocatePacman(pacman));
        }
    }

    void RelocateTreasureChest()
    {
        if (treasureChest != null && gameObject.CompareTag("Boss"))
        {
            treasureChest.transform.position = transform.position + new Vector3(0, 1, 0); // Move chest to boss position
            treasureChest.SetActive(true);
        }
    }

    public void onAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerAttack());

    }

    IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(1f);
    }

    public void onGuardButton()
    {

    }

    IEnumerator PlayerGuard()
    {
        yield return new WaitForSeconds(2f);
    }

    public void onSkillButton()
    {

    }

    IEnumerator PlayerSkill()
    {
        yield return new WaitForSeconds(2f);
    }
    public void onRunButton()
    {

    }
    IEnumerator PlayerRun()
    {
        yield return new WaitForSeconds(2f);
    }

}
