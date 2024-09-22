using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] generateFalloffMap(int size, float fallOffRange, float fallOffOffset)
    {
        float[,] map = new float[size,size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value, 10/fallOffRange, fallOffOffset);
            }

        return map;
    }

    public static float Evaluate(float value, float a, float b)
    {
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b*value, a));
    }

    public static float[,] GetFallOffMapAtCoord(float[,] fallOffMap, Vector2 topLeftCoord, Vector2 currentCoord, int mapSize)
    {
        float[,] newFallOffMap = new float[mapSize, mapSize];
        Vector2 offset = currentCoord - topLeftCoord;
        //Debug.Log(offset);
        for (int i = 0; i < mapSize; i++)
            for (int j = 0; j < mapSize; j++)
            {
                //Debug.Log(i + " " + j);
                newFallOffMap[i, j] = fallOffMap[(int)offset.x * (mapSize - 3) + i, -(int)offset.y * (mapSize - 3) + j];
            }

        return newFallOffMap;
    }
}
