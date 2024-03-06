// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject("GameManager");
                    instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }

    private HashSet<Vector2Int> savedFloor;
    public HashSet<Vector2Int> SavedFloor { get { return savedFloor; } }

    private List<DungeonObjects> dungeonObjects;
    public List<DungeonObjects> DungeonObjectsList { get { return dungeonObjects; } }

    private Vector3 playerPosition;

    private Vector3 chestPosition;

    public Vector3 ChestPosition { get { return chestPosition; } }

    public Vector3 PlayerPosition { get { return playerPosition; } }

    private void Awake()
    {
        savedFloor = new HashSet<Vector2Int>();
        dungeonObjects = new List<DungeonObjects>();
        playerPosition = Vector3.zero;
        chestPosition = Vector3.zero;
    }

    public void RemoveObjectAtPosition(Vector2 position)
    {
        dungeonObjects.RemoveAll(obj => obj.position == position);
    }

    public void UpdateSavedFloor(HashSet<Vector2Int> floor)
    {
        savedFloor = floor;
    }

    public void AddDungeonObject(DungeonObjects dungeonObject)
    {
        dungeonObjects.Add(dungeonObject);
    }

    public void ClearDungeonObjects()
    {
        dungeonObjects.Clear();
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public void UpdateChestPosition(Vector3 position)
    {
        chestPosition = position;
    }
}
