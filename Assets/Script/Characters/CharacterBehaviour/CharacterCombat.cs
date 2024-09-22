using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : NetworkBehaviour
{
    private float knockBackDuration = 0.2f;

    public CharacterStats myStats { get; private set; }
    protected AttackPos attackPos { get; private set; }
    public string targetTag { get; protected set; }

    protected CharacterControl characterControl { get; private set; }

    public delegate void OnInitiateAttack();
    public OnInitiateAttack initiateAttackCallback;
    

    protected virtual void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        attackPos = GetComponentInChildren<AttackPos>();
        characterControl = GetComponent<CharacterControl>();
    }

    protected virtual void Start()
    {
        SetTargetTag();
        myStats.takingDamageCallBack += GetKnockBack;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void GetKnockBack(GameObject source)
    {
        Vector3 generalDirection = this.transform.position - source.transform.position;
        generalDirection = new Vector3(generalDirection.x, 0, generalDirection.z);
        Vector3 knockDirection = Vector3.Normalize(generalDirection);
        float knockDistance = source.GetComponent<CharacterStats>().statData.attackKnockbackDistance.GetValue();
        //Debug.Log(gameObject.name + "got knock back");
        characterControl.KnockBack(knockDirection, knockDistance, knockBackDuration);
    }

    public virtual void DealDamage(CharacterStats target)
    {
        if (target != null)
        {
            Debug.Log("Dealing Damage");
            target.TakeDamageRpc(NetworkObject, myStats.statData.damage.GetValue());
        }
        else
        {
            Debug.Log("Target is null");
        }
    }

    protected virtual void Attack()
    {
        characterControl.StartAttack();
        if (initiateAttackCallback != null)
            initiateAttackCallback();
    }

    protected virtual void SetTargetTag()
    {
        // Set target tag
    }
}
