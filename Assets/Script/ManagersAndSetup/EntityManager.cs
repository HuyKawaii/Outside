using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    #region || ----- Singleton ----- ||
    public static EntityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    
    private float manageTimer;
    private float manageTimerMax = 5.0f;

    void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        ManageEnemy();
        //if (manageTimer < 0)
        //{
        //    ManageResource();
        //    manageTimer = manageTimerMax;
        //}
        //else
        //{
        //    manageTimer -= Time.deltaTime;
        //}
       
    }

    private void ManageEnemy()
    {
        foreach (Transform enemy in SetupSingleton.Instance.enemyHolder)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.UpdateDistanceToPlayer();

            if (enemyController.distance > TerrainLoader.Instance.renderDistance)
            {
                enemy.gameObject.SetActive(false);
            }
            else
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }

    public void ManageResource()
    {
        foreach (Transform enemy in SetupSingleton.Instance.resourceHolder)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.UpdateDistanceToPlayer();

            if (enemyController.distance > TerrainLoader.Instance.renderDistance)
            {
                enemy.gameObject.SetActive(false);
            }
            else
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
