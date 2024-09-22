using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;
    [SerializeField]
    private Transform enemyGroup;
    private float minDistance = 20.0f;
    private float maxDistance = 25.0f;

    [SerializeField] private List<GameObject> spawnList = new List<GameObject>();
    private int maxNumberOfEnemy = 6;
    private float enemySpawnTimer;
    private float enemySpawnRateMin = 5.0f;
    private float enemySpawnRateMax = 20.0f;

    [SerializeField] private bool isSpawning;
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


    void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (!isSpawning) return;
        if (DayNightManager.instance.timePeriod == DayNightManager.TimeOfDay.NightTime)
        {
            if (enemySpawnTimer <= 0)
            {
                //Debug.Log("Spawning enemies. Enemy count: " + entityList.Count);
                if (SetupSingleton.Instance.enemyHolder.childCount < maxNumberOfEnemy)
                {
                    int enemyToSpawn = Random.Range(0, spawnList.Count);
                    Vector3 spawnPosition = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
                    NavMeshHit hit;
                    int attemp = 0;
                    while (!NavMesh.SamplePosition(PlayerManager.Instance.player.transform.position + spawnPosition, out hit, spawnList[enemyToSpawn].GetComponent<NavMeshAgent>().height * 2, 1) && attemp < 10)
                    {
                        attemp++;
                    }

                    if (attemp == 10)
                    {
                        //Debug.Log("Cant place enemy on NavMesh Surface");
                        return;
                    }

                    //NavMeshAgent newEnemy;
                    //newEnemy = Instantiate(spawnList[enemyToSpawn], PlayerManager.Instance.player.transform.position + spawnPosition, spawnList[enemyToSpawn].transform.rotation).GetComponent<NavMeshAgent>();
                    //newEnemy.Warp(hit.position);
                    //newEnemy.transform.GetComponent<NetworkObject>().Spawn();
                    //enemyList.Add(newEnemy.transform);
                    MultiplayerManager.Instance.SpawnEnemyMultiplayer(spawnList[enemyToSpawn], hit.position);
                    enemySpawnTimer = Random.Range(enemySpawnRateMin, enemySpawnRateMax);
                }
            }
            else
            {
                enemySpawnTimer -= Time.deltaTime;
            }
        }
    }

    public void SpawnEnemyByIndex(int entityIndex, Vector3 spawnPosition, Transform parent = null, float health = 0)
    {
        GameObject newEntity = Instantiate(MultiplayerManager.Instance.GetEnemyFromIndex(entityIndex), spawnPosition, Quaternion.identity);


        if (newEntity == null)
        {
            Debug.Log("Null entity");
            return;
        }

        Debug.Log("Spawning enemy");
        newEntity.GetComponent<NetworkObject>().Spawn();

        if (parent == null)
            newEntity.transform.parent = SetupSingleton.Instance.enemyHolder.transform;
        else
            newEntity.transform.parent = parent;

        NavMeshAgent enemyAI = newEntity.GetComponent<NavMeshAgent>();
        if (enemyAI != null)
            enemyAI.Warp(spawnPosition);

        if (health > 0) 
            newEntity.GetComponent<EnemyStats>().SetHealth(health);
    }

    public void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition)
    {
        GameObject newEntity = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, SetupSingleton.Instance.enemyHolder);

        if (newEntity == null)
        {
            Debug.Log("Null entity");
            return;
        }

        NavMeshAgent enemyAI = newEntity.GetComponent<NavMeshAgent>();
        if (enemyAI != null)
            enemyAI.Warp(spawnPosition);
    }
}
