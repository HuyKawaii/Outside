using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSpawner : MonoBehaviour
{
    #region Singleton
    public static StructureSpawner Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
   
    [SerializeField]
    private StructureListSO structureList;
    private LayerMask ignoreLayer;
    private float raycastRange = 100.0f;

    private void Start()
    {
        ignoreLayer = LayerMask.NameToLayer("Ground");
        ignoreLayer = ~ignoreLayer;
    }

    public void SpawnStructures()
    {
        foreach (var structure in structureList.structureList)
        {
            for (int i = 0; i < structure.structureCount; i++)
            {
                float randX = Random.Range(TerrainLoader.Instance.worldSize/2 * structure.minSpawnRange, TerrainLoader.Instance.worldSize / 2 * structure.maxSpawnRange);
                float randY = Random.Range(TerrainLoader.Instance.worldSize/2 * structure.minSpawnRange, TerrainLoader.Instance.worldSize / 2 * structure.maxSpawnRange);

                MultiplayerManager.Instance.SpawnStructureMutiplayer(structure, SamplePosition(new Vector2(randX, randY)));
            }
        }
    }

    private Vector3 SamplePosition(Vector2 position)
    {
        //Debug.Log(position);
        RaycastHit hit;
        Vector3 rayPosition = new Vector3(position.x, -10 + raycastRange, position.y);
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, raycastRange, ignoreLayer))
        {

            Debug.DrawRay(rayPosition, Vector3.up * raycastRange, Color.blue, 300.0f, false);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(rayPosition, Vector3.up * raycastRange, Color.red, 300.0f, false);
            return Vector3.zero;
        }
    }
}
