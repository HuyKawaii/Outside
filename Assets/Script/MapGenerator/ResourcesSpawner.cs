using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ResourcesSpawner : MonoBehaviour
{
    public static ResourcesSpawner Instance;
    public NoiseSettings noiseSettings;
    public ResourcesListSO resourcesSettings;
    private LayerMask ignoreLayer;
    private float randomHorizontalOffsetRange = 3.0f;
    private float spawnVerticalOffset = 20.0f;
    private int spawnScanInterval = 10;
    private int spawnVeinInternal = 3;
    private float spawnOffset = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        noiseSettings.seed = GameManager.Instance.worldSeed;
        ignoreLayer = LayerMask.NameToLayer("Ground");
        ignoreLayer = ~ignoreLayer;
        //Debug.Log("Layer: " + ignoreLayer);
    }

    public void SpawnResources(TerrainChunk terrainChunk)
    {
        int noiseMapSize = terrainChunk.heightMap.values.GetLength(0) - 2;
        float[,] noise = NoiseGenerator.GenerateNoiseMap(noiseMapSize, noiseMapSize, terrainChunk.sampleCenter, noiseSettings);
        for (int k = 0; k < resourcesSettings.resourceList.Count; k++)
        {
            ResourcesSettings resources = resourcesSettings.resourceList[k];
            //Debug.Log(resources.name);
            //Transform resourceHolder = new GameObject(resources.name).transform;
            //resourceHolder.transform.parent = terrainChunk.meshObject.transform;
            int resourceCounter = 0;
            bool finishedSpawning = false;
            for (int i = 0; i < noiseMapSize; i += spawnScanInterval)
            {

                for (int j = 0; j < noiseMapSize; j += spawnScanInterval)
                    if (resources.isNoiseInRange(noise[i, j]))
                    {
                        RandomlySpawn(resources, terrainChunk, new Vector2(i, j), ref resourceCounter);
                        if (resourceCounter >= resources.maxPerChunk)
                        {
                            finishedSpawning = true;
                            break;
                        }
                    }
                if (finishedSpawning)
                    break;
            }
        }
    }

    public void RandomlySpawn(ResourcesSettings resources, TerrainChunk terrainChunk, Vector2 indexes, ref int resourceCounter)
    {
        
        float horizontalIndexFrom = Mathf.Max(indexes.x - resources.veinSize / 2.0f * spawnVeinInternal, 0);
        float horizontalIndexTo = Mathf.Min(indexes.x + resources.veinSize / 2.0f * spawnVeinInternal, terrainChunk.heightMap.values.GetLength(0) - 2);
        float verticalIndexFrom = Mathf.Max(indexes.y - resources.veinSize / 2.0f * spawnVeinInternal, 0);
        float verticalIndexTo = Mathf.Min(indexes.y + resources.veinSize / 2.0f * spawnVeinInternal, terrainChunk.heightMap.values.GetLength(0) - 2);
        for (int i = Mathf.CeilToInt(horizontalIndexFrom); i < horizontalIndexTo; i += spawnVeinInternal)
        {
            for (int j = Mathf.CeilToInt(verticalIndexFrom); j < verticalIndexTo; j += spawnVeinInternal)
            {
                if (terrainChunk.IsPositionUnderSand(i + 1, j + 1))
                    continue;
                if (UtilFunc.Roll(resources.spawnChance))
                {
                    Vector3 spawnPosition = GetPositionFromIndex(terrainChunk.heightMap.values, new Vector2(i, j), terrainChunk.chunkPosition, terrainChunk.chunkUnscaleSize, terrainChunk.chunkSize);
                    //Debug.Log("Spawn position: " + spawnPosition);
                    if (spawnPosition != Vector3.zero)
                    {
                        MultiplayerManager.Instance.SpawnResourceMultiplayer(resources, spawnPosition);
                        resourceCounter++;
                    }
                }
            }
        }
    }

    private Vector3 GetPositionFromIndex(float[,] heightMap, Vector2 indices, Vector2 chunkPosition, float chunkUnscaleSize, float chunkSize)
    {
        float positionX = (indices.x - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.x;
        float positionZ = -(indices.y - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.y;
        Vector3 spawnPosition = new Vector3(positionX + Random.Range(0, randomHorizontalOffsetRange), heightMap[(int)indices.x + 1, (int)indices.y + 1] + spawnVerticalOffset / 2, positionZ + Random.Range(0, randomHorizontalOffsetRange));
        //Debug.Log("Ray cast from: " + spawnPosition);
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, spawnVerticalOffset, ignoreLayer))
        {

            Debug.DrawRay(spawnPosition, Vector3.down * spawnVerticalOffset, Color.blue, 300.0f, false);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(spawnPosition, Vector3.down * spawnVerticalOffset, Color.red, 300.0f, false);
            return Vector3.zero;
        }
    }

    private Vector2 Position2DRandomOffset(Vector2 position)
    {
        return new Vector2(position.x + Random.Range(0, randomHorizontalOffsetRange), position.y + Random.Range(0, randomHorizontalOffsetRange));
    }

    public GameObject SpawnResourcesByIndex(int resourceIndex, Vector3 spawnPosition, float health = 0)
    {
        GameObject newResource = Instantiate(MultiplayerManager.Instance.GetResourceFromIndex(resourceIndex).prefab, spawnPosition, Quaternion.identity);

        if (newResource == null)
        {
            Debug.LogWarning("Trying to spawn null resources.");
            return null;
        }

        newResource.GetComponent<NetworkObject>().Spawn();

        //Debug.Log("Index:" + resourceIndex);
        if (resourceIndex >= 3)
            newResource.GetComponent<ResourceController>().SetReference(MultiplayerManager.Instance.GetResourceFromIndex(resourceIndex));

        //Debug.Log(SetupSingleton.Instance.pickupHolder);
        if (resourceIndex < 3)
            newResource.transform.parent = SetupSingleton.Instance.pickupHolder;
        else
        {
            ResourceStats resourceStats = newResource.GetComponent<ResourceStats>();
            if (resourceStats != null && health > 0)
                resourceStats.SetHealth(health);
            newResource.transform.parent = SetupSingleton.Instance.resourceHolder;

        }

        return newResource;
    }
}
