using System.Collections;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public float speed = 3f;
    public float edgeLength = 3f;

    private Vector3 startPosition;
    private Vector3[] directions;
    private int currentDirectionIndex = 0;
    private bool canMove = true;

    private void Start()
    {
        startPosition = transform.position;
        InitializeDirections();
        StartCoroutine(MoveInSquare());
    }

    void InitializeDirections()
    {
        directions = new Vector3[] {
            new Vector3(2, 0, 0) * edgeLength, // kanan
            new Vector3(0, 0, 2) * edgeLength, // maju
            new Vector3(-2, 0, 0) * edgeLength, // kiri
            new Vector3(0, 0, -2) * edgeLength  // mundur
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