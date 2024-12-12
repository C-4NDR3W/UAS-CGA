using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of the ghost
    private Vector3 direction;  // Direction of movement

    void Start()
    {
        // Generate a random initial direction
        direction = GetRandomDirection();
    }

    void Update()
    {
        // Move the ghost in the current direction
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Reflect the current direction based on the collision normal
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
    }

    Vector3 GetRandomDirection()
    {
        // Generate a random direction vector
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        return new Vector3(x, 0, z).normalized;
    }
}
