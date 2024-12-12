using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject pacmanPrefab;
    public GameObject[] ghostPrefabs; // Array untuk semua prefab ghost
    public int numberOfGhosts = 4;

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Rule stairsRoom;
    public Vector2 offset;

    private List<Cell> board;

    public bool isBattleScene = false; 

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
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    void Start()
    {
        MazeGenerator();
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

        if (cell - size.x >= 0 && !board[cell - size.x].visited)
        {
            neighbors.Add(cell - size.x);
        }

        if (cell + size.x < board.Count && !board[cell + size.x].visited)
        {
            neighbors.Add(cell + size.x);
        }

        if ((cell + 1) % size.x != 0 && !board[cell + 1].visited)
        {
            neighbors.Add(cell + 1);
        }

        if (cell % size.x != 0 && !board[cell - 1].visited)
        {
            neighbors.Add(cell - 1);
        }

        return neighbors;
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
        SpawnGhosts();
    }

    void PlaceStairsRoom()
    {
        if (stairsRoom == null || stairsRoom.room == null)
        {
            Debug.LogError("Stairs room prefab is not assigned!");
            return;
        }

        List<int> eligibleCells = new List<int>();
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].visited && i != startPos)
            {
                eligibleCells.Add(i);
            }
        }

        if (eligibleCells.Count == 0)
        {
            Debug.LogError("No eligible cells for stairs room!");
            return;
        }

        int randomIndex = eligibleCells[Random.Range(0, eligibleCells.Count)];
        Vector2Int position = new Vector2Int(randomIndex % size.x, randomIndex / size.x);

        var stairsRoomInstance = Instantiate(stairsRoom.room, new Vector3(position.x * offset.x, 0, -position.y * offset.y), Quaternion.identity, transform);
        stairsRoomInstance.name = "Stairs Room";

        RoomBehaviour roomBehaviour = stairsRoomInstance.GetComponent<RoomBehaviour>();
        if (roomBehaviour != null)
        {
            bool[] status = new bool[4];
            status[0] = randomIndex - size.x >= 0 && board[randomIndex - size.x].visited;
            status[1] = randomIndex + size.x < board.Count && board[randomIndex + size.x].visited;
            status[2] = (randomIndex + 1) % size.x != 0 && board[randomIndex + 1].visited;
            status[3] = randomIndex % size.x != 0 && board[randomIndex - 1].visited;

            roomBehaviour.UpdateRoom(status);
        }
        else
        {
            Debug.LogError("Stairs room prefab is missing a RoomBehaviour component!");
        }
    }

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

    void SpawnGhosts()
        {
            List<Vector3> spawnPositions = new List<Vector3>();

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Cell currentCell = board[(i + j * size.x)];
                    if (currentCell.visited)
                    {
                        spawnPositions.Add(new Vector3(i * offset.x, 0, -j * offset.y));
                    }
                }
            }

            for (int k = 0; k < numberOfGhosts; k++)
            {
                if (spawnPositions.Count == 0) break;

                int randomIndex = Random.Range(0, spawnPositions.Count);
                Vector3 spawnPosition = spawnPositions[randomIndex];
                spawnPositions.RemoveAt(randomIndex);

                if (ghostPrefabs.Length > 0)
                {
                    int randomGhostIndex = Random.Range(0, ghostPrefabs.Length);
                    GameObject ghost = Instantiate(ghostPrefabs[randomGhostIndex], spawnPosition, Quaternion.identity);

                    GhostBehaviour ghostBehaviour = ghost.GetComponent<GhostBehaviour>();
                    if (ghostBehaviour != null)
                    {
                        if (isBattleScene)
                        {
                            ghostBehaviour.enabled = false;
                        }
                    }
                }
            }
        }
    }