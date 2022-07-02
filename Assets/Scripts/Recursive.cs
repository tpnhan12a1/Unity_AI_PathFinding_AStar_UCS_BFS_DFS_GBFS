using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recursive : Maze
{
    public override void Generate()
    {
        Generate(1,1);
        base.Generate();
    }
    void Generate(int x, int z)
    {
        if (CountSquareNeighbours(x, z) >= 2) return;
        _map[x, z] = 0;

        _directions.Shuffle();

        for(int i=0;i < _directions.Count;i++)
        {
            Generate(x + _directions[i].x, z + _directions[i].z);
        }    
    }
}
