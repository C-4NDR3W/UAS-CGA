using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public enum UIPanelState
{
    Default,
    Skills
}


public class BattleController : MonoBehaviour
{
    //public static BattleController Instance;

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
    public TMP_Text enemyHealth;
    public TMP_Text turn;

    public GameObject gameOverUIPanel;
    private GhostBehaviour currentGhost;

    public InGameAudio inGameAudio;
    private UIPanelState currentUIPanelState = UIPanelState.Default;
    public bool isIntimidated = false;
    public bool isInversed = false;
    public bool isSelectingNewSkill = false;
    private void Start()
    {
        if (battleUIPanel == null)
        {
            battleUIPanel = InGameUI.Instance.battleUIPanel;
            doctorUIPanel = InGameUI.Instance.doctorUIPanel;
            gameOverUIPanel = InGameUI.Instance.gameOverUIPanel;
        }

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }

        dialogBox = battleUIPanel.transform.Find("Player TextBox").gameObject;
        dialogText = dialogBox.transform.Find("Player Text").GetComponent<TMP_Text>();

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        enemyHealth = battleUIPanel.transform.Find("Enemy Health").GetComponent<TMP_Text>();
        turn = battleUIPanel.transform.Find("Turn").GetComponent<TMP_Text>();



        inGameAudio = FindObjectOfType<InGameAudio>();
    }

    public void InitializeBattleUI()
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
            if (isIntimidated)
            {
                enemyStats.TakeIntimidateDamage(PlayerStats.Instance.attackPower);
                isIntimidated = false;
            }
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

        UpdateUI();

        if (enemyStats != null && enemyStats.isDead())
        {
            state = BattleState.WIN;
            StartCoroutine(EndBattle());
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

        UpdateUI();

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnSkillButton()
    {
        if (currentUIPanelState == UIPanelState.Default)
        {
            OpenSkillPanel();
            inGameAudio.PlayClickSound();
        }
        else
        {
            OpenDefaultPanel();
        }
    }

    private void OpenSkillPanel(bool selectNewSkill = false)
    {
        currentUIPanelState = UIPanelState.Skills;

        List<Skill> skills = PlayerStats.Instance.skills;
        if (selectNewSkill)
        {
            attackButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 0 ? skills[0].name + skills[0].tier : "Skill 1";
            skillButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 1 ? skills[1].name + skills[1].tier : "Skill 2";
            guardButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 2 ? skills[2].name + skills[2].tier : "Skill 3";
            runButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 3 ? skills[3].name + skills[3].tier : "Back";

            attackButton.onClick.RemoveAllListeners();
            skillButton.onClick.RemoveAllListeners();
            guardButton.onClick.RemoveAllListeners();
            runButton.onClick.RemoveAllListeners();

            attackButton.onClick.AddListener(() => ForgetSkill(0));
            skillButton.onClick.AddListener(() => ForgetSkill(1));
            guardButton.onClick.AddListener(() => ForgetSkill(2));
            runButton.onClick.AddListener(() => ForgetSkill(3));
        }

        attackButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 0 ? skills[0].name + skills[0].tier : "Skill 1";
        skillButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 1 ? skills[1].name + skills[1].tier : "Skill 2";
        guardButton.GetComponentInChildren<TMP_Text>().text = skills.Count > 2 ? skills[2].name + skills[2].tier : "Skill 3";
        runButton.GetComponentInChildren<TMP_Text>().text = "Back";

        attackButton.onClick.RemoveAllListeners();
        skillButton.onClick.RemoveAllListeners();
        guardButton.onClick.RemoveAllListeners();
        runButton.onClick.RemoveAllListeners();

        attackButton.onClick.AddListener(() => UseSkill(0));
        skillButton.onClick.AddListener(() => UseSkill(1));
        guardButton.onClick.AddListener(() => UseSkill(2));
        runButton.onClick.AddListener(OpenDefaultPanel);
    }

    private void ForgetSkill(int skillIndex)
    {
        List<Skill> skills = PlayerStats.Instance.skills;
        if (skillIndex < skills.Count)
        {
            Skill selectedSkill = skills[skillIndex];
            StartCoroutine(PlayerForgetSKill(selectedSkill));
        }
    }
    IEnumerator PlayerForgetSKill(Skill skill)
    {
        yield return new WaitForSeconds(1f);

    }
    private void OpenDefaultPanel()
    {
        // Switch back to the default panel
        currentUIPanelState = UIPanelState.Default;

        attackButton.GetComponentInChildren<TMP_Text>().text = "Attack";
        skillButton.GetComponentInChildren<TMP_Text>().text = "Skill";
        guardButton.GetComponentInChildren<TMP_Text>().text = "Guard";
        runButton.GetComponentInChildren<TMP_Text>().text = "Run";

        attackButton.onClick.RemoveAllListeners();
        skillButton.onClick.RemoveAllListeners();
        guardButton.onClick.RemoveAllListeners();
        runButton.onClick.RemoveAllListeners();

        attackButton.onClick.AddListener(OnAttackButton);
        skillButton.onClick.AddListener(OnSkillButton);
        guardButton.onClick.AddListener(OnGuardButton);
        runButton.onClick.AddListener(OnRunButton);
    }
    private void UseSkill(int skillIndex)
    {
        List<Skill> skills = PlayerStats.Instance.skills;
        if (skillIndex < skills.Count)
        {
            Skill selectedSkill = skills[skillIndex];
            StartCoroutine(PlayerUseSkill(selectedSkill));
        }
    }

    IEnumerator PlayerUseSkill(Skill skill)
    {
        dialogBox.SetActive(true);

        // Check if the skill is on cooldown
        if (skill.currentCooldown > 0)
        {
            dialogText.text = $"Cannot use {skill.name}! Cooldown: {skill.currentCooldown} turns left.";
            yield return new WaitForSeconds(1.5f);
            dialogBox.SetActive(false);
            yield break;
        }

        dialogText.text = $"Player uses {skill.name}!";
        yield return new WaitForSeconds(1f);

        // Handle skill-specific effects
        if (skill.name == "Intimidate")
        {
            isIntimidated = true;
            PlayerStats.Instance.UseSkill(skill, enemyStats);
            dialogText.text = $"Enemies will take more damage the next time you attack!";
            yield return new WaitForSeconds(1f);
        }
        else if (skill.name == "Inverse")
        {
            isInversed = true;
            PlayerStats.Instance.UseSkill(skill, enemyStats);
            dialogText.text = $"Next Enemy action will be reversed!";
            yield return new WaitForSeconds(1f);
        }
        else
        {
            // Use the skill and update UI accordingly
            PlayerStats.Instance.UseSkill(skill, enemyStats);
        }

        yield return new WaitForSeconds(1.5f);

        // Update UI and dialog box
        UpdateUI();
        dialogBox.SetActive(false);

        // Return to the default panel
        OpenDefaultPanel();
        if (enemyStats != null && enemyStats.isDead())
        {
            state = BattleState.WIN;
            StartCoroutine(EndBattle());
        }
        else
        {
            StartCoroutine(EnemyTurn());
            Debug.Log("Transferring to Enemy Turn: " + state);
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
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
        }

        UpdateUI();
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        UpdateUI();

        yield return new WaitForSeconds(1f);


        /// Determine the enemy's action based on conditions
        float missingHpPercentage = (float)(enemyStats.maxHp - enemyStats.currentHp) / enemyStats.maxHp;
        float healChance = 0f;

        if (missingHpPercentage >= 0.7f) // 70% missing HP
        {
            healChance = 0.3f; // Higher chance to heal
        }
        else if (missingHpPercentage >= 0.15f) // 15% missing HP
        {
            healChance = 0.1f; // Lower chance to heal
        }
        bool playerIsGuarding = isGuarding;

        // Adjust guard break chance
        float guardBreakChance = playerIsGuarding ? 0.15f : 0.1f;

        // Randomly decide the action
        float actionRoll = Random.value;
        if (actionRoll < healChance)
        {
            if (isInversed)
            {
                enemyStats.TakeDamage(enemyStats.HealAmount());
                isInversed = false;
                dialogBox.SetActive(true);
                dialogText.text = "Enemy healed itself!. But it was Inversed!";
            }
            else
            {
                enemyStats.HealEnemy();
                dialogBox.SetActive(true);
                dialogText.text = "Enemy healed itself!";
            }

            yield return new WaitForSeconds(1.5f);

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
        else if (actionRoll < guardBreakChance) // Guard break with a dynamic low chance
        {
            if (isInversed)
            {
                int damage = enemyStats.GuardBreak(isGuarding);
                enemyStats.TakeDamage(damage);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Guard Break while Inversed!";
                isInversed = false;
            }
            else
            {
                int damage = enemyStats.GuardBreak(isGuarding);
                PlayerStats.Instance.TakeDamage(damage, false);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Guard Break!";

            }

            yield return new WaitForSeconds(1.5f);

            if (dialogBox != null)
            {
                dialogBox.SetActive(false);
            }
        }
        else // Default action is attack
        {
            if (isInversed)
            {
                int damage = enemyStats.attackPower;
                PlayerStats.Instance.InversedHeal(damage);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy attacked while Inversed! You healed instead";
                isInversed = false;
            }
            else
            {
                PlayerStats.Instance.TakeDamage(enemyStats.attackPower, playerIsGuarding);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy attacked!";
            }

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
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            Debug.Log("Player's Turn");
        }
        PlayerStats.Instance.ReduceCooldowns();
        UpdateUI();
        isGuarding = false;
    }
    private IEnumerator EndBattle()
    {
        if (state == BattleState.WIN)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Pacman Wins!";
            yield return new WaitForSeconds(1f); // Wait for 1 second

            Debug.Log("player wins!");
            PlayerStats.Instance.AddCoins(1);
            PlayerStats.Instance.AddExperience(enemyStats.getXP());
            if (PlayerStats.Instance.skills.Count < 3)
            {
                PlayerStats.Instance.RewardSkillAfterBattle();
            }
            else
            {
                isSelectingNewSkill = true;
                dialogText.text = "You cannot hold more than 3 skills, please select one to forget.";
                yield return new WaitForSeconds(1f);
                OpenSkillPanel(isSelectingNewSkill);
            }
        }
        else if (state == BattleState.LOSE)
        {
            Debug.Log("Player Loses!");
            if (PlayerStats.Instance.currentHealth <= 0)
            {
                gameOverUIPanel.SetActive(true);
            }
        }
        // Reset skill cooldowns
        foreach (Skill skill in PlayerStats.Instance.skills)
        {
            skill.currentCooldown = 0;
        }

        battleUIPanel.SetActive(false);
        GhostBehaviour ghost = FindObjectOfType<GhostBehaviour>();
        if (ghost != null)
        {
            currentGhost.OnBattleEnd();
            currentGhost = null;
        }

        // Reset battle states
        state = BattleState.START;
        doctorUIPanel.SetActive(true);

        // Relocate Pacman and reset movement
        ResetPacmanPosition();

        // Reset enemy stats
        if (enemyStats != null)
        {
            enemyStats = null;
        }

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
            dialogText.text = string.Empty;
        }

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

    public IEnumerator StartBattle(EnemyStats enemy, GhostBehaviour ghost)
    {
        enemyStats = enemy;
        state = BattleState.START;

        battleUIPanel.SetActive(true);
        doctorUIPanel.SetActive(false);

        yield return new WaitForEndOfFrame();

        state = BattleState.PLAYERTURN;
        Debug.Log("Player Turn Starts");
        Debug.Log("State changed to: " + state);

        this.currentGhost = ghost;

        InitializeBattleUI();
        UpdateUI();
    }

    public void SetupBattle(GhostBehaviour ghost)
    {
        enemyStats = gameObject.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            int playerLevel = PlayerStats.Instance.level; // Get player's level
            enemyStats.Initialize(playerLevel);
        }

        StartCoroutine(StartBattle(enemyStats, ghost));
    }

    private void UpdateUI()
    {
        // Update turn text
        switch (state)
        {
            case BattleState.PLAYERTURN:
                turn.text = "Player";
                break;
            case BattleState.ENEMYTURN:
                turn.text = "Enemy";
                break;
            default:
                turn.text = "Battle Start";
                break;
        }

        // Update enemy health text
        if (enemyStats != null)
        {
            enemyHealth.text = $" {enemyStats.currentHp}/{enemyStats.maxHp} ";
        }
    }
}
