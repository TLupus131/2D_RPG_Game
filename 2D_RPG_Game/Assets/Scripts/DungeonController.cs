using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DungeonController : RandomWalkGenerator
{
    [SerializeField]
    private int minRoomWidth = 10, minRoomHeight = 10;
    [SerializeField]
    private int dungeonWidth = 70, dungeonHeight = 70;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 2;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField]
    private GameObject gold, chest, enemy;
    [SerializeField]
    private Tilemap floorTile;
    
    private Vector2Int firstRoomCenter;
    private Vector2Int lastRoomCenter;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dungeon")
        {
            if (tilemapVisualizer != null)
            {
                RunProcedureGeneration();
                SetPlayerPositionToFirstRoomCenter();
                SetChestPositionToLastRoomCenter();
            }
        }
    }

    protected override void RunProcedureGeneration()
    {
        tilemapVisualizer.Clear();
        CreateRooms();
    }

    private void SetPlayerPositionToFirstRoomCenter()
    {
        var player = FindObjectOfType<PlayerController_Dungeon>();
        if (player != null)
        {
            player.transform.position = new Vector3(firstRoomCenter.x, firstRoomCenter.y, player.transform.position.z);
        }
    }

    private void SetChestPositionToLastRoomCenter()
    {
        if (chest != null)
        {
            chest.transform.position = new Vector3(lastRoomCenter.x, lastRoomCenter.y, chest.transform.position.z);
        }
    }

    public void CreateRooms()
    {
        var roomList = ProceduralGenerator.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomList);
        }
        else
        {
            floor = CreateSimpleRooms(roomList);
        }

        if (roomList.Count > 0)
        {
            firstRoomCenter = (Vector2Int)Vector3Int.RoundToInt(roomList[0].center);
            lastRoomCenter = (Vector2Int)Vector3Int.RoundToInt(roomList.Last().center);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        if (roomList.Count > 0)
        {
            int count = 0;
            foreach (var room in roomList)
            {
                List<Vector2Int> nonWallPositions = GetNonWallPositions(room);
                int numberOfObjectsToPlace = Random.Range(1, 3);
                int numberOfEnemy = Random.Range(1, 2);
                for (int i = 0; i < numberOfObjectsToPlace; i++)
                {
                    int randomIndex = Random.Range(0, nonWallPositions.Count);
                    Vector2Int objectPosition = nonWallPositions[randomIndex];
                    GameObject objectPrefab = gold;
                    tilemapVisualizer.PaintSingleObject(objectPrefab, objectPosition);
                    nonWallPositions.RemoveAt(randomIndex);
                }
                for (int i = 0; i < numberOfEnemy; i++)
                {   
                    if (count > 0)
                    {
                        int randomIndex = Random.Range(0, nonWallPositions.Count);
                        Vector2Int objectPosition = nonWallPositions[randomIndex];
                        GameObject objectPrefab = enemy;
                        tilemapVisualizer.PaintSingleObject(objectPrefab, objectPosition);
                        nonWallPositions.RemoveAt(randomIndex);
                    }
                }
                if (count == 0)
                {
                    int randomIndex2 = Random.Range(0, nonWallPositions.Count);
                    Vector2Int objectPosition2 = nonWallPositions[randomIndex2];
                    gold.transform.position = new Vector2(objectPosition2.x, objectPosition2.y);
                    nonWallPositions.RemoveAt(randomIndex2);
                    count++;
                }
            }
            enemy.SetActive(false);
        }
    }

    List<Vector2Int> GetNonWallPositions(BoundsInt roomBounds)
    {
        List<Vector2Int> nonWallPositions = new List<Vector2Int>();

        for (int x = roomBounds.xMin + 1; x < roomBounds.xMax - 1; x++)
        {
            for (int y = roomBounds.yMin + 1; y < roomBounds.yMax - 1; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0); 

                if (floorTile.HasTile(cellPosition))
                {
                    Vector2Int position = new Vector2Int(x, y);
                    nonWallPositions.Add(position);
                }
            }
        }

        return nonWallPositions;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            var roomBounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);
        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            newCorridor = IncreaseCorridorSizeByTwo(newCorridor.ToList());
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> IncreaseCorridorSizeByTwo(List<Vector2Int> corridor)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                for (int j = -2; j <= 2; j++)
                {
                    for (int k = -2; k <= 2; k++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(j, k));
                    }
                }
                previousDirection = directionFromCell;
            }
            else
            {
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
                newCorridor.Add(corridor[i - 1] + 2 * newCorridorTileOffset); 
            }
        }
        return newCorridor;
    }


    private Vector2Int GetDirection90From(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            return Vector2Int.right;
        }
        else if (direction == Vector2Int.down)
        {
            return Vector2Int.left;
        }
        else if (direction == Vector2Int.left)
        {
            return Vector2Int.up;
        }
        else if (direction == Vector2Int.right)
        {
            return Vector2Int.down;
        }
        else
        {
            return Vector2Int.zero;
        }
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2Int.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
