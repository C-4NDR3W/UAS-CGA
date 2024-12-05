using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject pacmanPrefab;
    void SpawnOrRelocatePacman()
    {
        GameObject existingPacman = GameObject.FindGameObjectWithTag("Pacman");
        Vector3 spawnPosition = GetSpawnPosition();

        if (existingPacman != null)
        {
            existingPacman.transform.position = spawnPosition;
        }
        else
        {
            GameObject newPacman = Instantiate(pacmanPrefab, spawnPosition, Quaternion.identity);
            newPacman.tag = "Pacman";
        }
    }

    Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    return new Vector3(i * offset.x, 0, -j * offset.y);
                }
            }
        }
        return Vector3.zero;
    }

    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }

    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Rule stairsRoom;
    public Vector2 offset;

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
    {

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }
                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += $" {i}-{j}";

                }
            }
        }
        PlaceStairsRoom();
        SpawnOrRelocatePacman();
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }

    void PlaceStairsRoom()
    {
        if (stairsRoom == null || stairsRoom.room == null)
        {
            Debug.LogError("Stairs room prefab is not assigned!");
            return;
        }

        // Find eligible cells for stairs placement
        List<int> eligibleCells = new List<int>();
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].visited && i != startPos) // Exclude the starting position
            {
                eligibleCells.Add(i);
            }
        }

        if (eligibleCells.Count == 0)
        {
            Debug.LogError("No eligible cells for stairs room!");
            return;
        }

        // Pick a random eligible cell
        int randomIndex = eligibleCells[Random.Range(0, eligibleCells.Count)];
        Vector2Int position = new Vector2Int(randomIndex % size.x, randomIndex / size.x);

        // Instantiate the stairs room
        var stairsRoomInstance = Instantiate(stairsRoom.room,
                                             new Vector3(position.x * offset.x, 0, -position.y * offset.y),
                                             Quaternion.identity, transform);
        stairsRoomInstance.name = "Stairs Room";

        // Get the RoomBehaviour and update doors/walls based on neighbors
        RoomBehaviour roomBehaviour = stairsRoomInstance.GetComponent<RoomBehaviour>();
        if (roomBehaviour != null)
        {
            bool[] status = new bool[4];

            // Check neighbors and update connections
            status[0] = randomIndex - size.x >= 0 && board[randomIndex - size.x].visited; // Up
            status[1] = randomIndex + size.x < board.Count && board[randomIndex + size.x].visited; // Down
            status[2] = (randomIndex + 1) % size.x != 0 && board[randomIndex + 1].visited; // Right
            status[3] = randomIndex % size.x != 0 && board[randomIndex - 1].visited; // Left

            roomBehaviour.UpdateRoom(status);

            Debug.Log($"Stairs Room placed with entrances: Up={status[0]}, Down={status[1]}, Right={status[2]}, Left={status[3]}");
        }
        else
        {
            Debug.LogError("Stairs room prefab is missing a RoomBehaviour component!");
        }
    }

}
