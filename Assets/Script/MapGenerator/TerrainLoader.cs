using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    public static TerrainLoader Instance { get; private set; }
    public float renderDistance { get; private set; }

    public int minWorldSize;
    public float worldSize;
    int chunkPerDimension;
    Vector2 topLeftChunkCoord;
    Vector2 bottomRightChunkCoord;

    const float chunkUpdateDistanceThreshold = 25f;
    float sqrChunkUpdateDistanceThreshold
    { get { return chunkUpdateDistanceThreshold * chunkUpdateDistanceThreshold; } }

    public Material mapMaterial;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public int colliderLodIndex;
    public LodInfo[] lodInfoList;

    float chunkSize;
    int chunkVisibleInViewDistance;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleChunkList = new List<TerrainChunk>();
    public Transform viewer;
    Vector2 viewerPosition;
    Vector2 viewerOldPosition;

    float[,] fallOffMap;

    public bool doLoadWorld; //Debug only

    private bool isWorldGenerating = false;
    public bool isWorldLoaded { private set; get; }

    [SerializeField] private Mesh grassMesh;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Material terrainMaterial;
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
        isWorldLoaded = false;
        viewer = PlayerManager.Instance.player.transform;
        chunkSize = meshSettings.meshWorldSize;
        chunkPerDimension = Mathf.CeilToInt(minWorldSize / chunkSize);
        worldSize = chunkSize * chunkPerDimension;
        int topLeftXChunkCoor = (chunkPerDimension - 1) / -2;
        int topLeftYChunkCoor = (chunkPerDimension - 1) / 2; ;
        int bottomRightXChunkCoor = topLeftXChunkCoor + chunkPerDimension - 1;
        int bottomRightYChunkCoor = topLeftYChunkCoor - chunkPerDimension + 1; 
        topLeftChunkCoord = new Vector2(topLeftXChunkCoor, topLeftYChunkCoor);
        bottomRightChunkCoord = new Vector2(bottomRightXChunkCoor, bottomRightYChunkCoor);

        fallOffMap = FalloffGenerator.generateFalloffMap((int)(chunkPerDimension * (meshSettings.verticesPerLine - 3) + 3), heightMapSettings.fallOffRange, heightMapSettings.fallOffOffset);

        renderDistance = lodInfoList[lodInfoList.Length - 1].visibleDistanceThreshold;
        chunkVisibleInViewDistance = Mathf.CeilToInt(renderDistance / chunkSize);
        viewerOldPosition = viewerPosition;

        if (!TerrainLoader.Instance.doLoadWorld) return;

        MultiplayerManager.Instance.SetWorldSeedMultiplayer();
    }

    private void Update()
    {
        if (viewer == null || viewer.transform == null)
        {
            Debug.LogWarning("Viewer is null");
            return;
        }

        if (!isWorldGenerating && GameManager.Instance.isWorldSeedSet)
        {
            GenerateWorld();
            isWorldGenerating = true;
        }

        viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z);

        //if (viewerPosition != viewerOldPosition)
        //{
        //    foreach (TerrainChunk chunk in visibleChunkList)
        //    {
        //        chunk.UpdateCollisionMesh();
        //    }
        //}

        if ((viewerPosition - viewerOldPosition).sqrMagnitude >= sqrChunkUpdateDistanceThreshold )
        {
            //Debug.Log("Update chunk: new position :" + viewerPosition + " old position: " + viewerOldPosition);
            viewerOldPosition = viewerPosition;
            UpdateVisibleChunk();
            if (NetworkManager.Singleton.IsServer)
                EntityManager.Instance.ManageResource();
        }

        foreach(TerrainChunk terrainChunk in visibleChunkList)
            terrainChunk.DrawGrassGPU();
    }

    bool isChunkInWorld(Vector2 coord)
    {
        if (coord.x >= topLeftChunkCoord.x && coord.x <= bottomRightChunkCoord.x && coord.y >= bottomRightChunkCoord.y && coord.y <= topLeftChunkCoord.y)
            return true;
        else
            return false;
    }

    public void UpdateVisibleChunk()
    {
        visibleChunkList.Clear();
        foreach (var entry in terrainChunkDictionary)
            entry.Value.SetVisisble(false);

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        //Debug.Log("Current chunk: " + currentChunkCoordX + ", " + currentChunkCoordY);
        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset++)
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset++)
            {
                Vector2 chunkCoord = new Vector2(xOffset + currentChunkCoordX, yOffset + currentChunkCoordY);
                if (isChunkInWorld(chunkCoord))
                {
                    //Debug.Log("Chunk: " + chunkCoord + " is in view");
                    if (terrainChunkDictionary.ContainsKey(chunkCoord))
                    {
                        terrainChunkDictionary[chunkCoord].UpdateChunk();
                        visibleChunkList.Add(terrainChunkDictionary[chunkCoord]);
                    }
                    //else
                    //{
                    //    TerrainChunk terrainChunk = new TerrainChunk(chunkCoord, meshSettings, heightMapSettings, transform, viewer, mapMaterial, lodInfoList, colliderLodIndex, FalloffGenerator.GetFallOffMapAtCoord(fallOffMap, topLeftChunkCoord, chunkCoord, meshSettings.verticesPerLine));
                    //    terrainChunkDictionary.Add(chunkCoord, terrainChunk);
                    //    terrainChunk.OnTerrainChunkVisible += AddChunkToVisibleList;
                    //    terrainChunk.LoadHeightMap();
                    //}
                }
            }
    }

    public void GenerateWorld()
    {
        heightMapSettings.noiseSettings.seed = GameManager.Instance.worldSeed;
        //Debug.Log("Generating world");
        for (int i = 0; i < chunkPerDimension; i++)
            for (int j = 0; j < chunkPerDimension; j++)
            {
                //Debug.Log("Generating a chunk");
                Vector2 chunkCoord = new Vector2(topLeftChunkCoord.x + i, topLeftChunkCoord.y - j);
                TerrainChunk terrainChunk = new TerrainChunk(chunkCoord, meshSettings, heightMapSettings, transform, viewer, mapMaterial, lodInfoList, colliderLodIndex, FalloffGenerator.GetFallOffMapAtCoord(fallOffMap, topLeftChunkCoord, chunkCoord, meshSettings.verticesPerLine), grassMesh, grassMaterial, terrainMaterial);
                terrainChunkDictionary.Add(chunkCoord, terrainChunk);
                terrainChunk.LoadWholeChunk();
            }


        if (GameManager.Instance.IsGameDataSet() && NetworkManager.Singleton.IsServer)
        {
            GenerateSavedResources();
            GenerateSavedPickup();
            GenerateSavedStructure();
            GenerateSavedEnemis();
            GenerateSavedPlaceable();
            GenerateSavedChest();
        }

    }

    public void CheckWorldLoadComplete()
    {
        //Debug.Log("Check world load complete");
        foreach (var terrainChunkEntry in terrainChunkDictionary)
        {
            if (!terrainChunkEntry.Value.hasColliderSet)
            {
                return;
            }
        }

        if (!GameManager.Instance.IsGameDataSet())
        {
            foreach (var terrainChunkEntry in terrainChunkDictionary)
            {
                if (NetworkManager.Singleton.IsServer)
                    ResourcesSpawner.Instance.SpawnResources(terrainChunkEntry.Value);
            }

            if (NetworkManager.Singleton.IsServer)
                StructureSpawner.Instance.SpawnStructures();
        }

        isWorldLoaded = true;
        Debug.Log("World load complete. Now updating chunk");
        UpdateVisibleChunk();
        SetupSingleton.Instance.surface.BuildNavMesh();
        GameManager.Instance.GameLoadComplete();
    }

    private void GenerateSavedResources()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach (EntityData entityData in enviromentalData.resourceDataList)
        {
            MultiplayerManager.Instance.SpawnResourceFromIndexMultiplayer(entityData.entityIndex, entityData.position, entityData.health);
        }
    }

    private void GenerateSavedPickup()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach (PickupData pickupData in enviromentalData.pickupDataList)
        {
            //Debug.Log("Item index is: " + pickupData.itemIndex);
            MultiplayerManager.Instance.DropItemInWorld(pickupData.position, pickupData.itemIndex, pickupData.itemCount);
        }
    }

    private void GenerateSavedStructure()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach(StructureData structureData in enviromentalData.structureDataList)
        {
            MultiplayerManager.Instance.SpawnStructureFromIndexMutiplayer(structureData.structureIndex, structureData.position);
        }
    }

    private void GenerateSavedEnemis()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach(EntityData enemyData in enviromentalData.enemyDataList)
        {
            MultiplayerManager.Instance.SpawnEnemyFromIndexMultiplayer(enemyData.entityIndex, enemyData.position, enemyData.health);
        }
    }

    private void GenerateSavedPlaceable()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach(EntityData placeableData in enviromentalData.placeableDataList)
        {
            MultiplayerManager.Instance.PlaceObjectInWorldMultiplayerByIndex(placeableData.entityIndex, placeableData.position);
        }
    }

    private void GenerateSavedChest()
    {
        EnviromentalData enviromentalData = GameManager.Instance.GetEnviromentalData();
        foreach (ChestData chestData in enviromentalData.chestDataList)
        {
            MultiplayerManager.Instance.PlaceChestInWorldMultiplayer(chestData.position, chestData.chestContent);
        }
    }
}

[System.Serializable]
public struct LodInfo
{
    [Range(0, MeshSettings.numberOfSupportedLod - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold
    { get { return visibleDistanceThreshold * visibleDistanceThreshold; } }
}