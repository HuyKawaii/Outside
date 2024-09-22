using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetupSingleton : MonoBehaviour
{
    #region Singleton
    public static SetupSingleton Instance;

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
    }
    #endregion

    public GameObject holderPrefab;
    public Transform enemyHolder;
    public Transform pickupHolder;
    public Transform resourceHolder;
    public Transform structureHolder;
    public Transform placeableHolder;

    public NavMeshSurface surface;
    public RectTransform toolTips;
    public AnimationCurve gotHitAnimationCurve;
    public Transform gameManagers;
    public LayerMask groundLayer;
    

    private void Start()
    {
        surface = GetComponentInChildren<NavMeshSurface>();
        gameManagers = GameObject.Find("GameManagers").transform;
        groundLayer = LayerMask.NameToLayer("Ground");
        MultiplayerManager.Instance.SpawnSetupHolder();
    }
}
