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
    // private bool isPacmanRelocating = false; // To prevent multiple simultaneous relocations
    public bool isBattle = false; // Bool to check if the battle is active
    private PlayerMovement playerMovement; // Reference to Pacman's movement script (PlayerMovement)
    private Animator pacmanAnimator; // Reference to Pacman's Animator component
    public GameObject battleUIPanel; // Reference to the UI Panel for the battle


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
        if (other.CompareTag("Pacman") && !isBattle)
        {
            isBattle = true;

            pacmanAnimator = other.GetComponent<Animator>();
            playerMovement = other.GetComponent<PlayerMovement>();

            BattleController battleController = FindObjectOfType<BattleController>();
            if (battleController != null)
            {
                Vector3 originalPosition = other.transform.position;
                Quaternion originalRotation = other.transform.rotation;

                playerMovement.enabled = false; // Disable movement
                pacmanAnimator.SetBool("isWalking", false); // Stop walking animation
                GhostBehaviour ghostBehaviour = gameObject.GetComponent<GhostBehaviour>();
                if (ghostBehaviour != null)
                {
                    ghostBehaviour.SetMovement(false);
                }

                // Set up battle state in the BattleController
                battleController.SetPlayerState(originalPosition, originalRotation, playerMovement);
                battleController.StartBattle(GetComponent<EnemyStats>());
                battleUIPanel.SetActive(true);
                battleController.initializeBattleUI();

                // Relocate Pacman and enemy to battle positions
                RelocateForBattle(other.gameObject);
            }
        }
    }

    private void RelocateForBattle(GameObject pacman)
    {
        pacman.transform.position = new Vector3(15.322f, 27.94f, 4.538f);
        pacman.transform.rotation = Quaternion.Euler(0f, 135f, 0f);

        transform.position = new Vector3(18.25f, 28.17f, 1.85f);
        transform.rotation = Quaternion.Euler(0f, -45f, 0f);
    }


    // private IEnumerator TemporarilyRelocatePacman(GameObject pacman)
    // {
    //     isPacmanRelocating = true;

    //     // Store the original position of Pacman
    //     originalPosition = pacman.transform.position;
    //     originalRotation = pacman.transform.rotation;


    //     // Disable Pacman's movement (if PlayerMovement is attached)
    //     if (playerMovement != null)
    //     {
    //         playerMovement.enabled = false;
    //     }
    //     else
    //     {
    //         Debug.LogError("PlayerMovement script not found on Pacman.");
    //     }

    //     // New position to relocate Pacman
    //     Vector3 newPosition = new Vector3(15.322f, 27.94f, 4.538f);

    //     pacman.transform.position = newPosition;
    //     pacman.transform.rotation = Quaternion.Euler(0f, 135f, 0f);

    //     Vector3 newGhostPosition = new Vector3(18.25f, 28.17f, 1.85f);
    //     Quaternion newGhostRotation = Quaternion.Euler(0f, -45f, 0f);
    //     gameObject.transform.position = newGhostPosition;
    //     gameObject.transform.rotation = newGhostRotation;
    //     GhostBehaviour ghostBehaviour = gameObject.GetComponent<GhostBehaviour>();
    //     if (ghostBehaviour != null)
    //     {
    //         ghostBehaviour.SetMovement(false);
    //     }

    //     yield return new WaitForSeconds(5f);

    //     pacman.transform.position = originalPosition;
    //     pacman.transform.rotation = originalRotation;

    //     // Re-enable Pacman's movement
    //     if (playerMovement != null)
    //     {
    //         playerMovement.enabled = true;
    //     }

    //     isBattle = false;
    //     battleUIPanel.SetActive(false);
    //     isPacmanRelocating = false;
    //     Destroy(gameObject);
    //     PlayerStats.Instance.AddCoins(1);
    // }

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

    public void OnBattleEnd()
    {
        // Stop any movement and cleanup logic for the ghost
        canMove = false;
        isBattle = false;

        // Optionally relocate or destroy the ghost
        Destroy(gameObject);
    }

    public void SetMovement(bool enable)
    {
        canMove = enable;
    }
}