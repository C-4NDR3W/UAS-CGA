using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public class BattleController : MonoBehaviour
{

    public GameObject treasureChest;
    public BattleState state;
    private EnemyStats enemyStats;
    private bool isGuarding = false; // Tracks if the player is guarding

    private void Start()
    {
        
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

        // After enemy's turn, switch to the player's turn
        state = BattleState.PLAYERTURN;
        Debug.Log("Player's Turn");
    }

    /*
    private void endbattle()
    {
        if (state == battlestate.win)
        {
            debug.log("player wins!");
            battleuipanel.setactive(false);
            destroy(gameobject); // destroy enemy object
            reward player
                playerstats.instance.addcoins(1);
        }
        else if (state == battlestate.lose)
        {
            debug.log("player loses!");
            handle game over logic here
            }

        isbattle = false;
        ispacmanrelocating = false;

        return pacman to the original position
            if (originalposition != vector3.zero)
        {
            playermovement.enabled = true;
            playermovement.transform.position = originalposition;
            playermovement.transform.rotation = originalrotation;
        }
    }
    */
}
