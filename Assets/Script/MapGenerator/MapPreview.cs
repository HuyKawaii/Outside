using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public enum DisplayMode { NoiseMode, MeshMode, FallOff }
    public DisplayMode displayMode;
    public bool autoUpdate;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;
    public Material terrainMaterial;

    [Range(0, MeshSettings.numberOfSupportedLod - 1)]
    public int editorPreviewLod = 0;

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        textureData.ApplyToMaterial(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HideOnPlay hideOnPlay = GetComponentInChildren<HideOnPlay>(true);
        if (hideOnPlay == null)
        {
            DrawMapInScene();
            //Debug.Log("Drawing");
        }
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInScene();
    }

    void OnTextureValueUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
    }



    public void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.onValuesUpdated -= OnValuesUpdated;
            meshSettings.onValuesUpdated += OnValuesUpdated;
        }

        if (heightMapSettings != null)
        {
            heightMapSettings.onValuesUpdated -= OnValuesUpdated;
            heightMapSettings.onValuesUpdated += OnValuesUpdated;
        }

        if (textureData != null)
        {
            textureData.onValuesUpdated -= OnTextureValueUpdated;
            textureData.onValuesUpdated += OnTextureValueUpdated;
        }

    }

    public void DrawMapInScene()
    {
        float[,] fallOffMap = FalloffGenerator.generateFalloffMap(meshSettings.verticesPerLine, heightMapSettings.fallOffRange, heightMapSettings.fallOffOffset);
        textureData.ApplyToMaterial(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.verticesPerLine, meshSettings.verticesPerLine, Vector2.zero, heightMapSettings, fallOffMap);
        MapPreview mapDisplay = FindObjectOfType<MapPreview>();

        if (mapDisplay == null)
            return;

        if (displayMode == DisplayMode.NoiseMode)
        {
            mapDisplay.DisplayMap(CreateTexture.CreateTextureFromNoiseMap(heightMap));
            textureRenderer.gameObject.SetActive(true);
            meshRenderer.gameObject.SetActive(false);
        }
        else if (displayMode == DisplayMode.MeshMode)
        {
            mapDisplay.DisplayMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLod));
            if (textureRenderer != null)
                textureRenderer.gameObject.SetActive(false);
            meshRenderer.gameObject.SetActive(true);
        }
            
        else if (displayMode == DisplayMode.FallOff)
        {
            mapDisplay.DisplayMap(CreateTexture.CreateTextureFromNoiseMap(new HeightMap(new float[0,0], FalloffGenerator.generateFalloffMap(meshSettings.verticesPerLine, heightMapSettings.fallOffRange, heightMapSettings.fallOffOffset), 0, 1)));
            textureRenderer.gameObject.SetActive(true);
            meshRenderer.gameObject.SetActive(false);   
        }
    }

    public void DisplayMap(Texture2D texture)
    {

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DisplayMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = null;
    }
}
