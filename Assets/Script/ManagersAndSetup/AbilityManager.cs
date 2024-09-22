using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AbilityManager : NetworkBehaviour
{
    #region Singleton
    public static AbilityManager Instance;
   
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

        abilityUnlock = new bool[System.Enum.GetValues(typeof(Ability)).Length];
    }
    #endregion

    public bool[] abilityUnlock
    { get; private set; }
}

public enum Ability
{
    Dash,
    FireBall,
}
