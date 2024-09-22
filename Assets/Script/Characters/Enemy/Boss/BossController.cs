using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossController : EnemyController
{
    private float dashingSpeed = 1000;
    private float angularSpeed = 120;
    private float acceleration = 8;
    private float stoppingDistance = 1.5f;

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Behaviour()
    {
    }

    public void BossDash(Vector3 destination)
    {
        agent.speed = dashingSpeed;
        agent.angularSpeed = dashingSpeed;
        agent.acceleration = dashingSpeed;
        agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.autoBraking = false;
        agent.stoppingDistance = 0;
        agent.destination = destination;
        ((BossCombat)characterCombat).specialAttackCol.enabled = true;
    }

    public void StopDash()
    {
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
        agent.autoBraking = true;
        agent.stoppingDistance = stoppingDistance;
        ((BossCombat)characterCombat).specialAttackCol.enabled = false;
    }
}
