using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public class BattleController : MonoBehaviour
{

    public GameObject treasureChest;
    public BattleState state;
    public GameObject battleUIPanel;
    private EnemyStats enemyStats;
    private bool isGuarding = false; // Tracks if the player is guarding
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private PlayerMovement playerMovement;

    public GameObject doctorUIPanel;
    public Button attackButton;
    public Button skillButton;
    public Button guardButton;
    public Button runButton;

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

    public void initializeBattleUI()
    {
        // Find the buttons in the Battle UI
        attackButton = battleUIPanel.transform.Find("Attack Button").GetComponent<Button>();
        skillButton = battleUIPanel.transform.Find("Skill Button").GetComponent<Button>();
        guardButton = battleUIPanel.transform.Find("Guard Button").GetComponent<Button>();
        runButton = battleUIPanel.transform.Find("Run Button").GetComponent<Button>();

        attackButton.onClick.AddListener(OnAttackButton);
        skillButton.onClick.AddListener(OnSkillButton);
        guardButton.onClick.AddListener(OnGuardButton);
        runButton.onClick.AddListener(OnRunButton);

        Debug.Log("Battle UI initialized and listeners attached.");
    }

    public void OnAttackButton()
    {
        Debug.Log("Player Attack Button");
        if (state != BattleState.PLAYERTURN)
        {
            Debug.Log("Cannot attack, wrong state: " + state);
            return;
        }
    }

    IEnumerator PlayerAttack()
    {
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(PlayerStats.Instance.attackPower);
        }
        yield return new WaitForSeconds(1f);

        if (enemyStats != null && enemyStats.isDead())
        {
            state = BattleState.WIN;
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void OnGuardButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerGuard());

    }

    IEnumerator PlayerGuard()
    {
        isGuarding = true;
        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnSkillButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerSkill());
    }

    IEnumerator PlayerSkill()
    {
        yield return new WaitForSeconds(1f);
    }
    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(PlayerRun());
    }
    IEnumerator PlayerRun()
    {
        float runChance;
        yield return new WaitForSeconds(1f);
        if (enemyStats.enemyLevel <= PlayerStats.Instance.level)
        {
            runChance = 0.8f;
        }
        else
        {
            runChance = 0.4f;
        }

        if (Random.Range(0.0f, 1.0f) < runChance)
        {
            state = BattleState.LOSE;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
        }
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        yield return new WaitForSeconds(1f);


        PlayerStats.Instance.TakeDamage(enemyStats.attackPower, isGuarding);
        isGuarding = false;

        yield return new WaitForSeconds(1f);

        if (PlayerStats.Instance.currentHealth <= 0)
        {
            state = BattleState.LOSE;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            Debug.Log("Player's Turn");
        }
    }
    private void EndBattle()
    {
        if (state == BattleState.WIN)
        {
            Debug.Log("player wins!");
            PlayerStats.Instance.AddCoins(1);
        }
        else if (state == BattleState.LOSE)
        {
            Debug.Log("Player Loses!");
            if (PlayerStats.Instance.currentHealth <= 0)
            {
                //game over
            }
        }

        battleUIPanel.SetActive(false);
        GhostBehaviour ghost = FindObjectOfType<GhostBehaviour>();
        if (ghost != null)
        {
            ghost.OnBattleEnd();
        }

        // Reset battle states
        state = BattleState.START;
        doctorUIPanel.SetActive(true); // Hides the entire panel

        // Relocate Pacman and reset movement
        ResetPacmanPosition();
    }

    private void ResetPacmanPosition()
    {
        if (originalPosition != Vector3.zero)
        {
            playerMovement.enabled = true;
            playerMovement.transform.position = originalPosition;
            playerMovement.transform.rotation = originalRotation;
        }
    }

    public void SetPlayerState(Vector3 position, Quaternion rotation, PlayerMovement movement)
    {
        originalPosition = position;
        originalRotation = rotation;
        playerMovement = movement;
    }

    public void StartBattle(EnemyStats enemy)
    {
        enemyStats = enemy;
        state = BattleState.START;

        // Make sure the battle UI is active
        battleUIPanel.SetActive(true);

        // Disable the doctor UI (if it’s blocking the button)
        doctorUIPanel.SetActive(false);

        // Set the state to Player's turn
        state = BattleState.PLAYERTURN;
        Debug.Log("Battle Started!");

        // Initialize the battle UI if necessary
        initializeBattleUI();
    }

    public void SetupBattle()
    {
        enemyStats = gameObject.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            int playerLevel = PlayerStats.Instance.level; // Get player's level
            enemyStats.Initialize(playerLevel);
        }

        StartBattle(enemyStats);
    }
}
