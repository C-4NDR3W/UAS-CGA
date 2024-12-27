using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GhostBehaviour : MonoBehaviour
{
    public float speed = 3f;
    public float edgeLength = 3f;

    private Vector3 startPosition;
    private Vector3[] directions;
    private int currentDirectionIndex = 0;
    private bool canMove = true;

    private Vector3 originalPosition; // To store the original position of Pacman
    private Quaternion originalRotation; // To store the original rotation of Pacman
    private bool isPacmanRelocating = false; // To prevent multiple simultaneous relocations
    public bool isBattle = false; // Bool to check if the battle is active
    private PlayerMovement playerMovement; // Reference to Pacman's movement script (PlayerMovement)
    private Animator pacmanAnimator; // Reference to Pacman's Animator component
    public GameObject battleUIPanel; // Reference to the UI Panel for the battle
    public Button attackButton;
    public Button skillButton;
    public Button guardButton;
    public Button runButton;


    private void Start()
    {
        startPosition = transform.position + new Vector3(-2.0f, 0, 0); 
        InitializeDirections();
        StartCoroutine(MoveInSquare());

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
                HandleEnemyCollision(other.gameObject);
            }
            else if (gameObject.CompareTag("Boss"))
            {
                // Boss collision: 50 damage, 100 coins, spawn treasure chest
                //reminder change damage prolly for this
                HandleEnemyCollision(other.gameObject);
            }
        }
    }

    void HandleEnemyCollision(GameObject pacman)
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
                initializeBattleUI();
            }

            // Handle temporary relocation
            StartCoroutine(TemporarilyRelocatePacman(pacman));
        }
    }

    void initializeBattleUI()
    {
        // Find the buttons in the Battle UI
        attackButton = battleUIPanel.transform.Find("Attack Button").GetComponent<Button>();
        skillButton = battleUIPanel.transform.Find("Skill Button").GetComponent<Button>();
        guardButton = battleUIPanel.transform.Find("Guard Button").GetComponent<Button>();
        runButton = battleUIPanel.transform.Find("Run Button").GetComponent<Button>();

        Debug.Log("Battle UI initialized and listeners attached.");
    }

    private IEnumerator TemporarilyRelocatePacman(GameObject pacman)
    {
        isPacmanRelocating = true;

        // Store the original position of Pacman
        originalPosition = pacman.transform.position;
        originalRotation = pacman.transform.rotation;


        // Disable Pacman's movement (if PlayerMovement is attached)
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        else
        {
            Debug.LogError("PlayerMovement script not found on Pacman.");
        }

        // New position to relocate Pacman
        Vector3 newPosition = new Vector3(15.322f, 27.94f, 4.538f);

        pacman.transform.position = newPosition;
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

        yield return new WaitForSeconds(5f);

        pacman.transform.position = originalPosition;
        pacman.transform.rotation = originalRotation;

        // Re-enable Pacman's movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isBattle = false;
        battleUIPanel.SetActive(false);
        isPacmanRelocating = false;
        Destroy(gameObject);
        PlayerStats.Instance.AddCoins(1);
    }

        void InitializeDirections()
    {
        directions = new Vector3[] {
            new Vector3(2.6f, 0, 0) * edgeLength, // kanan
            new Vector3(0, 0, 2.6f) * edgeLength, // maju
            new Vector3(-2.6f, 0, 0) * edgeLength, // kiri
            new Vector3(0, 0, -2.6f) * edgeLength  // mundur
        };
    }

    private IEnumerator MoveInSquare()
    {
        Vector3 nextPosition = startPosition;

        while (true)
        {
            if (!canMove)
            {
                yield return null;
                continue;
            }

            nextPosition += directions[currentDirectionIndex];
            while (Vector3.Distance(transform.position, nextPosition) > 0.1f)
            {
                if (!canMove) break;

                Vector3 direction = (nextPosition - transform.position).normalized; 
                transform.position += direction * speed * Time.deltaTime;

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);

                yield return null;
            }

            currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
        }
    }

    public void SetMovement(bool enable)
    {
        canMove = enable;
    }
}