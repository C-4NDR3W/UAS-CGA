using System.Collections;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f;
    public enum MovementPattern { FollowPacman, Square }
    public MovementPattern movementPattern = MovementPattern.Square;

    private Vector3 targetPosition;
    private GameObject pacman;
    private Coroutine movementCoroutine;
    private Vector3[] squareWaypoints;
    private int currentWaypointIndex = 0;

    void Start()
    {
        pacman = GameObject.FindGameObjectWithTag("Pacman");
        squareWaypoints = new Vector3[]
        {
            new Vector3(-6, 0, -6),
            new Vector3(-6, 0, 6),
            new Vector3(6, 0, -6),
            new Vector3(6, 0, 6)
        };

        Quaternion rotation = Quaternion.Euler(0, -40, 0);
        for (int i = 0; i < squareWaypoints.Length; i++)
        {
            squareWaypoints[i] = rotation * squareWaypoints[i];
        }

        if (movementPattern == MovementPattern.Square)
        {
            movementCoroutine = StartCoroutine(MoveInSquare());
        }
    }

    void Update()
    {
        if (pacman != null)
        {
            if (movementPattern == MovementPattern.Square && movementCoroutine == null)
            {
                movementCoroutine = StartCoroutine(MoveInSquare());
            }
        }
    }

    IEnumerator MoveInSquare()
    {
        while (true)
        {
            targetPosition = transform.position + squareWaypoints[currentWaypointIndex];
            currentWaypointIndex = (currentWaypointIndex + 1) % squareWaypoints.Length;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
