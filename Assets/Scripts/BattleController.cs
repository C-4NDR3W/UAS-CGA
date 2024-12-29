using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
    public GameObject dialogBox;
    public TMP_Text dialogText;
    public Button attackButton;
    public Button skillButton;
    public Button guardButton;
    public Button runButton;
    public Button playerButton;
    
    public TMP_Text skillButtonText;

    public InGameAudio inGameAudio;
    private void Start()
    {
        if (battleUIPanel == null)
        {
            battleUIPanel = InGameUI.Instance.battleUIPanel;
            doctorUIPanel = InGameUI.Instance.doctorUIPanel;
        }

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }

        dialogBox = battleUIPanel.transform.Find("Player TextBox")?.gameObject;
        dialogText = dialogBox.transform.Find("Player Text")?.GetComponent<TMP_Text>();

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        inGameAudio = FindObjectOfType<InGameAudio>();
    }

    public void initializeBattleUI()
    {
        // Find the buttons in the Battle UI
        attackButton = battleUIPanel.transform.Find("Buttons/Attack Button").GetComponent<Button>();
        skillButton = battleUIPanel.transform.Find("Buttons/Skill Button").GetComponent<Button>();
        guardButton = battleUIPanel.transform.Find("Buttons/Guard Button").GetComponent<Button>();
        runButton = battleUIPanel.transform.Find("Buttons/Run Button").GetComponent<Button>();

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
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerAttack());
        Debug.Log("Player Attacked! " + state);
    }

    IEnumerator PlayerAttack()
    {
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(PlayerStats.Instance.attackPower);
            if (dialogText != null)
            {
                dialogBox.SetActive(true);
                dialogText.text = "Player attacks the enemy!";
            }
        }
        yield return new WaitForSeconds(1.5f);

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        if (enemyStats != null && enemyStats.isDead())
        {
            state = BattleState.WIN;
            EndBattle();
        }
        else
        {
            StartCoroutine(EnemyTurn());
            Debug.Log("Transferring to Enemy Turn: " + state);
        }
    }

    public void OnGuardButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerGuard());
    }

    IEnumerator PlayerGuard()
    {
        isGuarding = true;
        if (dialogText != null)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Player is guarding!";
        }
        yield return new WaitForSeconds(1.5f);

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnSkillButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerSkill());
    }

    IEnumerator PlayerSkill()
    {   
        if (dialogText != null)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Player uses a skill!";
        }
        yield return new WaitForSeconds(1.5f);

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
    
    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerRun());
    }
    IEnumerator PlayerRun()
    {
        if (dialogText != null)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Player attempts to run!";
        }
        float runChance;
        yield return new WaitForSeconds(1.5f);
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
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


        /// Determine the enemy's action based on conditions
        float missingHpPercentage = (float)(enemyStats.maxHp - enemyStats.currentHp) / enemyStats.maxHp;
        bool shouldHeal = missingHpPercentage >= 0.15f && enemyStats.currentHp < enemyStats.maxHp;
        bool shouldReallyHeal = missingHpPercentage >= 0.6f && enemyStats.currentHp < enemyStats.maxHp;
        bool playerIsGuarding = isGuarding;

        // Adjust guard break chance
        float guardBreakChance = playerIsGuarding ? 0.15f : 0.1f;

        // Randomly decide the action
        float actionRoll = Random.value; // Returns a value between 0 and 1
        if (shouldHeal && actionRoll < 0.1f)
        {
            enemyStats.HealEnemy();
            dialogBox.SetActive(true);
            dialogText.text = "Enemy healed itself!";
            
            yield return new WaitForSeconds(1.5f);
            
            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
        else if (shouldReallyHeal && actionRoll < 0.33f)
        {
            enemyStats.HealEnemy();
            dialogBox.SetActive(true);
            dialogText.text = "Enemy healed itself!";

            yield return new WaitForSeconds(1.5f);

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
        else if (actionRoll < guardBreakChance) // Guard break with a dynamic low chance
        {
            int damage = enemyStats.GuardBreak(isGuarding);
            PlayerStats.Instance.TakeDamage(damage, false);
            dialogBox.SetActive(true);
            dialogText.text = "Enemy used Guard Break!";

            yield return new WaitForSeconds(1.5f);

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
        else // Default action is attack
        {
            PlayerStats.Instance.TakeDamage(enemyStats.attackPower, playerIsGuarding);
            dialogBox.SetActive(true);
            dialogText.text = "Enemy attacked!";

            yield return new WaitForSeconds(1.5f);

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f);

        // Check if the player is defeated
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

        isGuarding = false;
    }
    private void EndBattle()
    {
        if (state == BattleState.WIN)
        {
            Debug.Log("player wins!");
            PlayerStats.Instance.AddCoins(1);
        }
        else if (state == BattleState.LOSE) //TODO GAME OVER
        {
            Debug.Log("Player Loses!");
            if (PlayerStats.Instance.currentHealth <= 0)
            {
                //game over
                //game over UI
                //Retry or Main Menu
                //retry mybe
                // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        doctorUIPanel.SetActive(true);

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

    public IEnumerator StartBattle(EnemyStats enemy)
    {
        enemyStats = enemy;
        state = BattleState.START;

        battleUIPanel.SetActive(true);
        doctorUIPanel.SetActive(false);

        yield return new WaitForEndOfFrame();

        state = BattleState.PLAYERTURN;
        Debug.Log("Player Turn Starts");
        Debug.Log("State changed to: " + state);

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

        StartCoroutine(StartBattle(enemyStats));
    }
}
