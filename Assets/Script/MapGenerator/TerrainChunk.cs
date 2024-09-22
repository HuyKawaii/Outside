using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 10f;
    float sqrColliderGenerationDistanceThreshold
    { get { return colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold; } }

    Transform viewer;
    MeshSettings meshSettings;
    HeightMapSettings heightMapSettings;
    Bounds bounds;
    public GameObject meshObject
    { get; private set; }
    public Vector2 sampleCenter
    { get; private set; }
    public float chunkSize
    { get; private set; }
    public float chunkUnscaleSize
    { get; private set; }
    public Vector2 chunkPosition
    { get; private set; }
    public HeightMap heightMap
    { get; private set; }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    public MeshCollider meshCollider;

    bool heightMapReceived;

    LodInfo[] lodInfoList;
    LodMesh[] lodMeshes;
    int colliderLodIndex;
    int preLodIndex = -1;
    int lodIndex = -1;
    float maxViewDistance;
    float sqrMaxViewDistance;
    float[,] fallOffMap;
    public bool hasColliderSet { private set; get; }
    bool hasResourcesSpawned = false;

    Mesh grassMesh;
    Material grassMaterial;
    Material terrainMaterial;
    List<List<Matrix4x4>> matrices;

    public float maxHeight;
    public float sandMaxHeight;
    public float grassMaxHeight;
    public TerrainChunk(Vector2 chunkCoord, MeshSettings meshSettings, HeightMapSettings heightMapSettings, Transform parent, Transform viewer, Material mapMaterial, LodInfo[] lodInfoList, int colliderLodIndex, float[,] fallOffMap, Mesh grassMesh, Material grassMaterial, Material terrainMaterial)
    {
        //Debug.Log("Generating chunk");
        this.meshSettings = meshSettings;
        this.heightMapSettings = heightMapSettings;
        this.viewer = viewer;
        this.lodInfoList = lodInfoList;
        this.colliderLodIndex = colliderLodIndex;
        this.fallOffMap = fallOffMap;
        this.grassMesh = grassMesh;
        this.grassMaterial = grassMaterial;
        this.terrainMaterial = terrainMaterial;

        maxHeight = terrainMaterial.GetFloat("_MaxHeight");
        grassMaxHeight = terrainMaterial.GetFloat("_GrassMaxHeight");
        sandMaxHeight = terrainMaterial.GetFloat("_SandMaxHeight");
        chunkUnscaleSize = meshSettings.meshUnscaledSize;
        chunkSize = meshSettings.meshWorldSize;
        chunkPosition = chunkCoord * meshSettings.meshWorldSize;
        sampleCenter = chunkPosition / meshSettings.scale;
        bounds = new Bounds(chunkPosition, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshObject.layer = LayerMask.NameToLayer("Ground");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = mapMaterial;
        meshObject.transform.position = new Vector3(chunkPosition.x, 0, chunkPosition.y);
        meshObject.SetActive(false);
        meshObject.transform.parent = parent;

        lodMeshes = new LodMesh[lodInfoList.Length];
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i] = new LodMesh(lodInfoList[i].lod);
            lodMeshes[i].onMeshReceiveCallback += UpdateChunk;
            if (i == colliderLodIndex)
                lodMeshes[i].onMeshReceiveCallback += UpdateCollisionMesh;
        }

        maxViewDistance = lodInfoList[lodInfoList.Length - 1].visibleDistanceThreshold;
        sqrMaxViewDistance = maxViewDistance * maxViewDistance;
    }

    Vector2 viewerPosition
    { get { return new Vector2(viewer.position.x, viewer.position.z); } }

    public void LoadHeightMap()
    {
        DataThreadRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.verticesPerLine, meshSettings.verticesPerLine, sampleCenter, heightMapSettings, fallOffMap), OnHeightMapReceived);
    }

    public void OnHeightMapReceived(object heightMapObject)
    {
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;
        UpdateChunk();
    }

    public void UpdateChunk()
    { 
        if (heightMapReceived)
        {
            float sqrDistanceToViewer = bounds.SqrDistance(viewerPosition);
            if (TerrainLoader.Instance.isWorldLoaded)
            {
                if (sqrDistanceToViewer <= sqrMaxViewDistance)
                    SetVisisble(true);
                else
                    SetVisisble(false);
            }
            else
                SetVisisble(true);

            if (isChunkVisible())
            {
                lodIndex = 0;
                for (int i = 0; i < lodInfoList.Length - 1; i++)
                {
                    if (sqrDistanceToViewer > lodInfoList[i].sqrVisibleDistanceThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }

                if (lodIndex != preLodIndex)
                {
                    if (lodMeshes[lodIndex].hasMesh)
                    {
                        preLodIndex = lodIndex;
                        meshFilter.mesh = lodMeshes[lodIndex].mesh;
                    }
                    else if (!lodMeshes[lodIndex].hasRequestedMesh)
                        lodMeshes[lodIndex].RequestMeshData(heightMap, meshSettings);
                }
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasColliderSet)
        {
            float sqrDistanceToViewer = bounds.SqrDistance(viewerPosition);

            if (sqrDistanceToViewer < lodInfoList[colliderLodIndex].sqrVisibleDistanceThreshold)
                if (!lodMeshes[colliderLodIndex].hasRequestedMesh)
                    lodMeshes[colliderLodIndex].RequestMeshData(heightMap, meshSettings);

            //if (sqrDistanceToViewer < sqrColliderGenerationDistanceThreshold)
            //{
                if (lodMeshes[colliderLodIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLodIndex].mesh;
                    hasColliderSet = true;
                    
                    TerrainLoader.Instance.CheckWorldLoadComplete();
                }
            //}
            //SetupSingleton.Instance.surface.BuildNavMesh();
        }
    }

    public void SetVisisble(bool visisble)
    {
        //if (visisble == false)
        //    Debug.Log("Deactivate chunk");
        //if (!isChunkVisible())
        //{
            meshObject.SetActive(visisble);
            //SetupSingleton.instance.surface.BuildNavMesh();
        //}
    }

    public bool isChunkVisible()
    {
        return meshObject.activeSelf;
    }

    public void LoadWholeChunk()
    {
        //Debug.Log("Load whole chunk");
        DataThreadRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.verticesPerLine, meshSettings.verticesPerLine, sampleCenter, heightMapSettings, fallOffMap), OnHeightMapReceivedOnLoadWholeChunk);
    }

    public void OnHeightMapReceivedOnLoadWholeChunk(object heightMapObject)
    {
        //Debug.Log("Requesting Mesh");
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        for (int  i = 0; i < lodInfoList.Length; i++)
        {
            if (!lodMeshes[i].hasRequestedMesh)
            {
                lodMeshes[i].RequestMeshData(heightMap, meshSettings);
                lodMeshes[i].hasRequestedMesh = true;
            }
        }

        InstantiateMaxtrixForInstancing();
    }

    public Vector3 GetPositionFromIndex(int x, int y)
    {
        float positionX = (x - 1 - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.x;
        float positionZ = -(y - 1 - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.y;
        return new Vector3(positionX, heightMap.values[x, y], positionZ);
    }

    public float GetHeightPercentage(float height)
    {
        return height / maxHeight;
    }

    public bool IsPositionUnderSand(int x, int y)
    {
        Vector3 position = GetPositionFromIndex(x, y);
        float heightPercentage = GetHeightPercentage(position.y);
        return heightPercentage <= sandMaxHeight;
    }

    private Vector3 RandomOffset()
    {
        float offsetMax = 0.5f;
        return new Vector3(Random.Range(-offsetMax, offsetMax), 0, Random.Range(-offsetMax, offsetMax));
    }

    private void InstantiateMaxtrixForInstancing()
    {
        matrices = new List<List<Matrix4x4>>();

        List<Matrix4x4> newMatrixList = new List<Matrix4x4>();
        int count = 0;

        for (int i = 1; i < meshSettings.verticesPerLine - 1; i++)
            for (int j = 1; j < meshSettings.verticesPerLine - 1; j++)
            {
                if (Random.Range(0, 1.0f) > 0.3f)
                    continue;
                Vector3 position = GetPositionFromIndex(i, j);
                float percantageHeight = GetHeightPercentage(position.y);
                if (percantageHeight > grassMaxHeight || percantageHeight <= sandMaxHeight)
                    continue;
                position += RandomOffset();

                if (count == 1000)
                {
                    matrices.Add(newMatrixList);
                    count = 0;
                    newMatrixList = new List<Matrix4x4>();
                }
                newMatrixList.Add(Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * 50));
                count++;
            }
        matrices.Add(newMatrixList);
    }

    public void DrawGrassGPU()
    {
        if (!heightMapReceived || !isChunkVisible() || lodIndex != 0) return;
        foreach (var matrixList in matrices)
        {
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, matrixList, null, UnityEngine.Rendering.ShadowCastingMode.Off);
        }
    }
}

public class LodMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action onMeshReceiveCallback;

    public LodMesh(int lod)
    {
        this.lod = lod;
    }

    public void RequestMeshData(HeightMap heightMap, MeshSettings meshSettings)
    {
        //Debug.Log("Request mesh data");
        hasRequestedMesh = true;
        DataThreadRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        onMeshReceiveCallback();
    }
}
