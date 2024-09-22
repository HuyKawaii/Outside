using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, Vector2 sampleCenter, HeightMapSettings heightMapSetting, float[,] fallOffMap)
    {
        //Debug.Log("Generating height map");
        AnimationCurve threadSafe_heightCurve = new AnimationCurve(heightMapSetting.heightCurve.keys);
        float[,] noise = NoiseGenerator.GenerateNoiseMap(width, height, sampleCenter, heightMapSetting.noiseSettings);
        float[,] values = noise;
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

       for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (heightMapSetting.enableFallOff)
                    values[i, j] = Mathf.Clamp01(values[i, j] - fallOffMap[i, j]);
                values[i, j] = threadSafe_heightCurve.Evaluate(values[i, j]) * heightMapSetting.heightMultiplier;
                if (values[i, j] < minValue) minValue = values[i, j];
                if (values[i, j] > maxValue) maxValue = values[i, j];
            }

       return new HeightMap(noise, values, minValue, maxValue);

    }
}



public struct HeightMap
{
    public readonly float[,] noise;
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] noise, float[,] values, float minValue, float maxValue)
    {
        this.noise = noise;
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}