using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;       // Reference to the player's transform
    public Vector3 offset;         // Offset of the camera from the player
    public float distance = 3f;    // Default distance from the player
    public float minDistance = 1f; // Minimum zoom distance
    public float maxDistance = 5f; // Maximum zoom distance
    public float zoomSpeed = 2f;   // Speed at which the camera zooms in/out
    public float wallDistance = 0.5f; // Distance before zooming in when hitting the wall

    private float currentDistance; // Current camera distance from the player

    void Start()
    {
        // Initialize the current distance
        currentDistance = distance;
    }

    void LateUpdate()
    {
        bool isMovingForward = Input.GetKey(KeyCode.W);

        // Raycast to avoid clipping through walls
        RaycastHit hit;
        Vector3 direction = (player.position - transform.position).normalized;

        if (Physics.Raycast(player.position, direction, out hit, currentDistance))
        {
            // If hitting a wall, zoom in (reduce distance)
            if (isMovingForward)
            {
                currentDistance = Mathf.Lerp(currentDistance, Mathf.Max(minDistance, hit.distance - wallDistance), Time.deltaTime * zoomSpeed);
            }
        }
        else
        {
            if (!isMovingForward)
            {
                currentDistance = distance;
            }
        }

        // Apply the adjusted camera position based on the current distance
        Vector3 desiredPosition = player.position - direction * currentDistance + offset;
        transform.position = desiredPosition;
    }
}
