using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    private Vector3 originalPosition; // To store the original position of Pacman
    private Quaternion originalRotation; // To store the original rotation of Pacman
    private bool isPacmanRelocating = false; // To prevent multiple simultaneous relocations
    private PlayerMovement playerMovement; // Reference to Pacman's movement script (PlayerMovement)
    public bool isBattle = false; // Bool to check if the battle is active
    private Animator pacmanAnimator; // Reference to Pacman's Animator component
    public GameObject battleUIPanel; // Reference to the UI Panel for the battle


    private void Start()
    {
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman") && gameObject.CompareTag("Ghost"))
        {
            if (!isPacmanRelocating)
            {
                isBattle = true; // Set isBattle to true when Pacman collides with a ghost

                pacmanAnimator = other.GetComponent<Animator>();
                playerMovement = other.GetComponent<PlayerMovement>();

                if (battleUIPanel != null && isBattle == true)
                {
                    pacmanAnimator.SetBool("isWalking", false);
                    battleUIPanel.SetActive(true); // Show the battle UI panel
                }
                StartCoroutine(TemporarilyRelocatePacman(other.gameObject));
            }
        }
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


        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Check if the original position is valid and return Pacman to it
        if (originalPosition != Vector3.zero)
        {
            pacman.transform.position = originalPosition;
            pacman.transform.rotation = originalRotation;
        }

        // Re-enable Pacman's movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isBattle = false;
        battleUIPanel.SetActive(false);
        isPacmanRelocating = false;
        Destroy(gameObject);
    }
}
