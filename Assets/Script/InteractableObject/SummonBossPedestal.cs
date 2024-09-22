using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonBossPedestal : Structure
{
    private float minDistance = 3.0f;
    private float maxDistance = 10.0f;
    public GameObject boss;
    public Transform enemyGroup;
    public override void Interact(Transform player)
    {
        Vector3 spawnPosition;
        NavMeshHit hit;
        while (true)
        {
            spawnPosition = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
            int attemp = 0;
            while (!NavMesh.SamplePosition(player.transform.position + spawnPosition, out hit, boss.GetComponent<NavMeshAgent>().height * 5, 1) && attemp < 10)
            {
                attemp++;
                if (attemp == 10)
                    Debug.Log("Cant place enemy on NavMesh Surface");
                break;
            }
            if (attemp == 10)
                continue;
            break;
        }
        
        NavMeshAgent newEnemy;
        newEnemy = Instantiate(boss, player.transform.position + spawnPosition, boss.transform.rotation, enemyGroup).GetComponent<NavMeshAgent>();
        newEnemy.Warp(hit.position);
        Destroy(gameObject);
    }

    protected override void SetHoverText()
    {
        hoverText = "Summon boss";
    }
}
