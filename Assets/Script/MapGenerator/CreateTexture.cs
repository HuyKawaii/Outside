using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreateTexture
{
    public static Texture2D CreateTextureFromNoiseMap(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[x + y * width] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }
    public static Texture2D CreateTextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point; 
        texture.Apply();

        return texture;
    }
}
