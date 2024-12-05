using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;       // Reference to the player's transform
    public Vector3 offset;         // Offset of the camera from the player
    public float sensitivity = 300f; // Mouse sensitivity
    public float distance = 3f;    // Distance from the player

    private float pitch = 0f;      // Vertical rotation (up/down)
    private float yaw = 0f;        // Horizontal rotation (left/right)

    void Start()
    {
        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Adjust yaw and pitch
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Limit vertical rotation to avoid weird angles

        // Calculate the camera's direction and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direction = rotation * Vector3.forward; // Use a direction vector
        Vector3 desiredPosition = player.position - direction * distance + offset;

        // Raycast to avoid clipping through walls
        RaycastHit hit;
        if (Physics.Raycast(player.position, -direction, out hit, distance))
        {
            transform.position = player.position - direction * (hit.distance - 0.5f) + offset;
        }
        else
        {
            transform.position = desiredPosition;
        }

        // Set camera rotation to look at the player
        transform.LookAt(player.position + Vector3.up * 1.5f); // Focus slightly above the player's center

        // Rotate the player to match the camera's yaw
        player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

}
