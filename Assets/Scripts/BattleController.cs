using System.Collections;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    private Vector3 originalPosition; // To store the original position of Pacman
    private Quaternion originalRotation; // To store the original rotation of Pacman
    private bool isPacmanRelocating = false; // To prevent multiple simultaneous relocations
    private PlayerMovement playerMovement; // Reference to Pacman's movement script (PlayerMovement)

    private void Start()
    {
        // Try to get the PlayerMovement script attached to Pacman
        playerMovement = FindObjectOfType<PlayerMovement>(); // Adjust if PlayerMovement is on a different object
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pacman") && gameObject.CompareTag("Ghost"))
        {
            if (!isPacmanRelocating)
            {
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

        isPacmanRelocating = false;

        Destroy(gameObject);
    }
}
