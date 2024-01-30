using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected RandomWalkData randomWalkParameters;

    protected override void RunProcedureGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer); 
    }

    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkData parameters, Vector2Int position)
    {
        var curentPositions = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path =  ProceduralGenerator.SimpleRandomWalk(curentPositions, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
            {
                curentPositions = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));  
            }
        }
        return floorPositions;
    }

}
