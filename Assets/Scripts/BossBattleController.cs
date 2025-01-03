using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBattleController : MonoBehaviour //largely a copy of BattleController with special boss-specific actions 
//and smaller code overall(hopefully) //this did not happen
{ // yes i did not inherit BattleController

    private Vector3 originalPosition; // To store the original position of Pacman
    private Quaternion originalRotation; // To store the original rotation of Pacman
    public bool isBattle = false; // Bool to check if the battle is active
    private PlayerMovement playerMovement;
    private BossBehaviour currentBoss;
    private BossStats bossStats;
    public BattleState state;
    public GameObject battleUIPanel;

    public GameObject doctorUIPanel;
    public GameObject dialogBox;
    public TMP_Text dialogText;

    public Button attackButton;
    public Button skillButton;
    public Button guardButton;
    public Button runButton;

    public TMP_Text bossHealth;
    public TMP_Text turn;
    public InGameAudio inGameAudio;

    private UIPanelState currentUIPanelState = UIPanelState.Default;
    public GameObject gameOverUIPanel;

    private bool isIntimidated = false;
    private bool isInversed = false;
    private bool isSelectingNewSkill = false;
    private bool isGuarding = false;
    private bool hasNotReversal = true; //alt names: has Bursted (Anime FGC), has Full Countered, has LimitBreak'd, has Supernova'd
    private bool phase2 = false; //im sorry (not really) //god i wish im better at unity enough to modify the bossbattle music
                                 // proposed boss battle music: Otherworld (FFX), Holy Orders ~Be Just or Be Dead~, Any doom soundtrack (Cyberdemon, Gladiator(?))
                                 // i also wish i could modify a game over music (FFX Game Over)
                                 //Nerd fun fact, the Spectogram of Cyberdemon has a secret message
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

        bossHealth = battleUIPanel.transform.Find("Enemy Health").GetComponent<TMP_Text>();
        turn = battleUIPanel.transform.Find("Turn").GetComponent<TMP_Text>();

        inGameAudio = FindObjectOfType<InGameAudio>();
    }


    public void SetPlayerState(Vector3 position, Quaternion rotation, PlayerMovement movement)
    {
        originalPosition = position;
        originalRotation = rotation;
        playerMovement = movement;
    }

    public void SetupBattle(BossBehaviour boss)
    {
        // Get the GameObject of the Boss
        GameObject bossGameObject = boss.gameObject;

        // Attempt to retrieve the BossStats component from the Boss GameObject
        bossStats = bossGameObject.GetComponent<BossStats>();
        if (bossStats == null)
        {
            Debug.LogError("BossStats component not found on this GameObject!");
            return; // Exit if component is missing
        }

        int playerLevel = PlayerStats.Instance.level;
        bossStats.Initialize(playerLevel);

        StartCoroutine(StartBattle(bossStats, boss)); // Pass the local variable
    }

    public IEnumerator StartBattle(BossStats bossStatsComponent, BossBehaviour boss)
    {
        this.currentBoss = boss;
        state = BattleState.START; //ensures it starts on the START state
        isGuarding = false;
        isIntimidated = false;
        isInversed = false;
        isSelectingNewSkill = false; //reset everything
        hasNotReversal = true;
        phase2 = false;

        battleUIPanel.SetActive(true);
        doctorUIPanel.SetActive(false);

        // Reset UI components
        dialogBox.SetActive(false);
        dialogText.text = string.Empty;

        // Delay to ensure UI updates are visible
        yield return new WaitForEndOfFrame();

        state = BattleState.PLAYERTURN;
        Debug.Log("Player Turn Starts");
        Debug.Log("State changed to: " + state);

        InitializeBattleUI();
        OpenDefaultPanel(); // Ensure default panel is displayed
        UpdateUI();
    }

    public void InitializeBattleUI()
    {
        currentUIPanelState = UIPanelState.Default;
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

    private void UpdateUI()
    {
        // Update turn text
        switch (state)
        {
            case BattleState.PLAYERTURN:
                turn.text = "Player";
                break;
            case BattleState.ENEMYTURN:
                turn.text = "Boss";
                break;
            default:
                turn.text = "Battle Start";
                break;
        }

        // Update enemy health text
        if (bossStats != null)
        {
            bossHealth.text = $" {bossStats.currentHp}/{bossStats.maxHp} ";
        }
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
    public void OnGuardButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerGuard());
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
    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        inGameAudio.PlayClickSound();
        StartCoroutine(PlayerRun());
    }
    IEnumerator PlayerAttack()
    {
        if (bossStats != null)
        {
            if (isIntimidated)
            {
                bossStats.TakeIntimidateDamage(PlayerStats.Instance.attackPower);
                isIntimidated = false;
            }
            else
            {
                bossStats.TakeDamage(PlayerStats.Instance.attackPower);
            }
            if (dialogText != null)
            {
                dialogBox.SetActive(true);
                dialogText.text = "Player attacks the Boss!";
            }
        }
        yield return new WaitForSeconds(1.5f);

        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        UpdateUI();
        StartCoroutine(EnemyTurn()); //due to having phase 2, death check is moved
    }

    private void OpenSkillPanel(bool selectNewSkill = false)
    {
        currentUIPanelState = UIPanelState.Skills;
        List<Skill> skills = PlayerStats.Instance.skills;

        // Dynamically setup button text and listeners
        SetupButton(attackButton, skills, 0, selectNewSkill);
        SetupButton(skillButton, skills, 1, selectNewSkill);
        SetupButton(guardButton, skills, 2, selectNewSkill);
        if (selectNewSkill)
        {
            SetupButton(runButton, skills, 3, selectNewSkill);
        }
        else
        {
            SetupButton(runButton, skills, 3, selectNewSkill, isBackButton: true);
        }
    }

    private void SetupButton(Button button, List<Skill> skills, int index, bool selectNewSkill, bool isBackButton = false)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (isBackButton)
        {
            buttonText.text = "Back";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OpenDefaultPanel);
            return;
        }

        // Update button text based on available skills
        buttonText.text = index < skills.Count ? skills[index].name + " Tier " + skills[index].tier : $"Skill {index + 1}";
        button.onClick.RemoveAllListeners();

        // Assign listener based on mode
        if (selectNewSkill && index < skills.Count)
            button.onClick.AddListener(() => ForgetSkill(index));
        else if (index < skills.Count)
            button.onClick.AddListener(() => UseSkill(index));
    }

    private void ForgetSkill(int skillIndex)
    {
        List<Skill> skills = PlayerStats.Instance.skills;
        if (skillIndex < skills.Count)
        {
            Skill selectedSkill = skills[skillIndex];
            StartCoroutine(PlayerForgetSkill(selectedSkill, skills[3]));
        }
    }

    private IEnumerator PlayerForgetSkill(Skill oldSkill, Skill newSkill)
    {
        yield return new WaitForSeconds(1f); // Optional delay

        // Reference the skills list
        List<Skill> skills = PlayerStats.Instance.skills;

        // Find the index of the old skill
        int skillIndex = skills.IndexOf(oldSkill);

        if (skillIndex >= 0)
        {
            if (skillIndex == 3)
            {
                // If the skill to forget is at index 3, remove it entirely
                skills.RemoveAt(3);
            }
            else
            {
                // Otherwise, overwrite the old skill with the new skill
                skills[skillIndex] = newSkill;

                // Remove the old skill at index 3 after replacing
                if (skills.Count > 3)
                {
                    skills.RemoveAt(3); // ensure skills[3] doesnt exist
                }
            }
        }
        else
        {
            Debug.LogError("Skill to forget not found in the list.");
        }

        // Refresh the skill panel after the change
        OpenSkillPanel(true);

        isSelectingNewSkill = false;
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
            PlayerStats.Instance.UseSkill(skill, bossStats);
            dialogText.text = $"Enemies will take more damage the next time you attack!";
            yield return new WaitForSeconds(1f);
        }
        else if (skill.name == "Inverse")
        {
            isInversed = true;
            PlayerStats.Instance.UseSkill(skill, bossStats);
            dialogText.text = $"Next Enemy action will be reversed!";
            yield return new WaitForSeconds(1f);
        }
        else
        {
            // Use the skill and update UI accordingly
            PlayerStats.Instance.UseSkill(skill, bossStats);
        }

        yield return new WaitForSeconds(1.5f);

        // Update UI and dialog box
        UpdateUI();
        dialogBox.SetActive(false);

        // Return to the default panel
        OpenDefaultPanel();
        StartCoroutine(EnemyTurn()); //due to having phase 2, death check is added at start of EnemyTurn
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
        if (bossStats.level <= PlayerStats.Instance.level)
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

    private IEnumerator EndBattle()
    {
        BossBehaviour boss = FindObjectOfType<BossBehaviour>();
        if (state == BattleState.WIN)
        {
            dialogBox.SetActive(true);
            dialogText.text = "Pacman Wins!";
            yield return new WaitForSeconds(1f); // Wait for 1 second

            Debug.Log("player wins!");
            PlayerStats.Instance.AddCoins(bossStats.level);
            PlayerStats.Instance.AddExperience(bossStats.getXP());
            if (PlayerStats.Instance.skills.Count < 3)
            {
                PlayerStats.Instance.RewardSkillAfterBattle();
            }
            else
            {
                PlayerStats.Instance.RewardSkillAfterBattle(); // reward stuff specifically skill
                isSelectingNewSkill = true;
                List<Skill> skills = PlayerStats.Instance.skills;
                dialogText.text = $"You cannot hold more than 3 skills, please select one to forget. You will obtain {skills[3].name} Tier {skills[3].tier}";
                yield return new WaitForSeconds(3f);
                dialogBox.SetActive(false);
                OpenSkillPanel(isSelectingNewSkill);
                while (isSelectingNewSkill)
                {
                    yield return null; // Wait for the next frame
                }
            }

            if (boss != null)
            {
                boss.SpawnTreasure();
                boss.OpenStairs();
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
        if (boss != null)
        {
            currentBoss.OnBattleEnd();
            currentBoss = null;
        }

        // Reset battle states
        state = BattleState.START;
        doctorUIPanel.SetActive(true);

        // Relocate Pacman and reset movement
        ResetPacmanPosition();

        // Reset enemy stats
        if (bossStats != null)
        {
            bossStats = null;
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

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        if (bossStats.currentHp <= 0) //1st death check
        {
            if (!phase2)
            {
                phase2 = true;
                bossStats.Phase2();
                dialogBox.SetActive(true);
                dialogText.text = "Enemy enters Phase 2!";
                yield return new WaitForSeconds(1.5f);
                dialogBox?.SetActive(false);
            }
            else
            {
                state = BattleState.WIN;
                StartCoroutine(EndBattle());
                yield break; // End the turn early if the boss is dead
            }
        }
        UpdateUI();
        yield return new WaitForSeconds(1f);


        /// Determine the enemy's action based on conditions
        float missingHpPercentage = (float)(bossStats.maxHp - bossStats.currentHp) / bossStats.maxHp;
        float healChance = 0f;

        if (missingHpPercentage >= 0.7f) // 70% missing HP
        {
            healChance = 0.35f; // Higher chance to heal
        }
        else if (missingHpPercentage >= 0.15f) // 15% missing HP
        {
            healChance = 0.1f; // Lower chance to heal
        }
        bool playerIsGuarding = isGuarding; // this bool is purely for setting chance

        // Adjust guard break chance
        float guardBreakChance = playerIsGuarding ? 0.15f : 0.1f;
        float reversalChance = 0f;

        if (bossStats.currentHp <= bossStats.maxHp * 0.6f && bossStats.currentHp > bossStats.maxHp * 0.3f && hasNotReversal) // Between 50% and 30% HP
        {
            dialogBox.SetActive(true);
            dialogText.text = "Warning! The boss is trying to do something at low HP!";
            yield return new WaitForSeconds(2f);
            dialogBox?.SetActive(false);
        }
        if (hasNotReversal && bossStats.currentHp <= bossStats.maxHp * 0.3f) // 30% or lower health
        {
            reversalChance = 0.5f + ((bossStats.maxHp * 0.3f - bossStats.currentHp) / (bossStats.maxHp * 0.3f)) * 0.5f; // Gradually increase up to 1.0
        }

        // Randomly decide the action
        float actionRoll = Random.value;
        if (actionRoll < healChance)
        {
            // Heal logic
            if (isInversed)
            {
                bossStats.TakeDamage(bossStats.HealAmount());
                isInversed = false;
                dialogBox.SetActive(true);
                dialogText.text = "Enemy healed itself!. But it was Inversed!";
            }
            else
            {
                bossStats.HealEnemy();
                dialogBox.SetActive(true);
                dialogText.text = "Enemy healed itself!";
            }

            yield return new WaitForSeconds(1.5f);
            dialogBox?.SetActive(false);
        }
        else if (actionRoll < guardBreakChance + healChance) // Guard break with a dynamic low chance
        {
            // Guard break logic
            if (isInversed)
            {
                int damage = bossStats.GuardBreak(isGuarding);
                bossStats.TakeDamage(damage);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Guard Break while Inversed!";
                isInversed = false;
            }
            else
            {
                int damage = bossStats.GuardBreak(isGuarding);
                PlayerStats.Instance.TakeDamage(damage, false);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Guard Break!";
            }

            yield return new WaitForSeconds(1.5f);
            dialogBox?.SetActive(false);
        }
        else if (actionRoll < guardBreakChance + healChance + reversalChance) // Reversal attack
        {
            // Reversal logic
            hasNotReversal = false; // Ensure only one reversal per phase
            if (isInversed)
            {
                int damage = bossStats.attackPower;
                PlayerStats.Instance.InversedHeal(damage);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Reversal while Inversed! You healed instead!";
                isInversed = false;
            }
            else
            {
                PlayerStats.Instance.TakeDamage(bossStats.ReversalAttack(), playerIsGuarding); // Stronger attack
                dialogBox.SetActive(true);
                dialogText.text = "Enemy used Reversal!";
            }

            yield return new WaitForSeconds(1.5f);
            dialogBox?.SetActive(false);
        }
        else // Default action is attack
        {
            // Attack logic
            if (isInversed)
            {
                int damage = bossStats.attackPower;
                PlayerStats.Instance.InversedHeal(damage);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy attacked while Inversed! You healed instead!";
                isInversed = false;
            }
            else
            {
                PlayerStats.Instance.TakeDamage(bossStats.attackPower, playerIsGuarding);
                dialogBox.SetActive(true);
                dialogText.text = "Enemy attacked!";
            }

            yield return new WaitForSeconds(1.5f);
            dialogBox?.SetActive(false);
        }

        // Check if phase 2 should be triggered
        if (bossStats.currentHp <= 0 && !phase2) //due to Inverse, there is a 2nd death check
        {
            phase2 = true;
            bossStats.Phase2();
            dialogBox.SetActive(true);
            dialogText.text = "Enemy enters Phase 2!";

            yield return new WaitForSeconds(1.5f);
            dialogBox?.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

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

}