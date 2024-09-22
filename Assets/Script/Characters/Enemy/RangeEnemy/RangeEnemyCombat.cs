using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyCombat : EnemyCombat
{
    [SerializeField]
    private GameObject arrowPrefab;
   
    public void SpawnArrow()
    {
        MultiplayerManager.Instance.SpawnProjectileMultiplayer(arrowPrefab, attackPos.transform.position, enemyController.target.position);
    }

  
}
