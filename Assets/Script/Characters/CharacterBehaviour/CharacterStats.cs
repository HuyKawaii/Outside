using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterStats : NetworkBehaviour
{
    public CharacterStatData statData;
    public float health 
    { protected set; get; }

    public delegate void OnTakingDamage(GameObject damageSource);
    public OnTakingDamage takingDamageCallBack;
    public delegate void OnDead();
    public OnDead deadCallBack;

    protected virtual void Start()
    {
        health = statData.maxHealth.GetValue();    
    }

    public virtual void TakeDamage(GameObject damageSource, float damageTaken)
    {
        damageTaken = damageTaken * 100 / (statData.armor.GetValue() + 100); //damage caculator

        health -= damageTaken;
        if (health <= 0)
            OutOfHealth();

        Debug.Log(gameObject.name + "takes " + damageTaken + " damage");

        if (takingDamageCallBack != null)
            takingDamageCallBack.Invoke(damageSource);
    }

    [Rpc(SendTo.Owner)]
    public void TakeDamageRpc(NetworkObjectReference damageSourceNetworkObjectReference, float damageTaken)
    {
        Debug.Log("Running take damage RPC");
        damageSourceNetworkObjectReference.TryGet(out NetworkObject damageSourceNetworkObject);
        TakeDamage(damageSourceNetworkObject.gameObject, damageTaken);
        Debug.Log("Running take damage RPC 2");
    }

    public virtual void Heal()
    {
        health = statData.maxHealth.GetValue();
    }


    public virtual void Heal(float amount)
    {
        health += amount;
    }

    public virtual void OutOfHealth()
    {
        //Do something here
        if (deadCallBack != null)
            deadCallBack.Invoke();
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }
}
