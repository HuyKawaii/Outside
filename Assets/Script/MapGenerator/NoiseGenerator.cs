using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public enum NormalizeMode { Local, Global }

    public static float[,] GenerateNoiseMap(int width, int height, Vector2 sampleCenter, NoiseSettings noiseSettings)
    {
        float globalMaxHeight = 0;
        float amplitude = 1;
        float frequency;

        float[,] noiseMap = new float[width, height];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        
        System.Random prng = new System.Random(noiseSettings.seed);
        Vector2[] octavesOffsets = new Vector2[noiseSettings.occtaves];
        for (int i = 0; i < noiseSettings.occtaves; i++)
        {
            float offsetX = prng.Next(-1000, 1000) + noiseSettings.offset.x + sampleCenter.x / noiseSettings.scale;
            float offsetY = prng.Next(-1000, 1000) - noiseSettings.offset.y - sampleCenter.y / noiseSettings.scale;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);
            globalMaxHeight += amplitude;
            amplitude *= noiseSettings.persistance;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseHeight = 0;
                amplitude = 1;
                frequency = 1;

                for (int i = 0; i < noiseSettings.occtaves; i++)
                {
                    float sampleX = Mathf.Round(((x - width / 2) / noiseSettings.scale * 
                        frequency + octavesOffsets[i].x * frequency) * 100) / 100;
                    float sampleY = Mathf.Round(((y - height / 2) / noiseSettings.scale * 
                        frequency + octavesOffsets[i].y * frequency) * 100) / 100;
                    float perlinValue = Mathf.PerlinNoise(sampleX , sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseSettings.persistance;
                    frequency *= noiseSettings.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

                if (noiseSettings.normalizeMode == NormalizeMode.Global)
                {
                    noiseMap[x, y] = (noiseMap[x, y] + 1) / (2 * globalMaxHeight / 1.5f);
                    noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y], 0, int.MaxValue);
                }
            }
        }

        if (noiseSettings.normalizeMode == NormalizeMode.Local)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                
        return noiseMap;
    }
}
[System.Serializable]
public class NoiseSettings
{
    public NoiseGenerator.NormalizeMode normalizeMode;
    public int seed;
    public float scale = 20;
    public int occtaves = 5;
    [Range(0f, 1f)]
    public float persistance = 0.6f;
    public float lacunarity = 2;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        occtaves = Mathf.Max(occtaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}