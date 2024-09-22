using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ResourceCombat : EnemyCombat
{
    [SerializeField] private GameObject objectGraphic;
    private NetworkVariable<float> gotHitAnimationTimer = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private float gotHitAnimationMaxTime = 0.5f;
    private Vector3 minScale = new Vector3 (0.8f, 0.8f, 0.8f);
    public delegate void ResourceDeathCallback();
    public ResourceDeathCallback onResourceDeathCallback;
    protected override void Start()
    {
        base.Start();
        myStats.takingDamageCallBack += TriggerGotHitAnimation;
    }

    protected override void Update()
    {
        PlayGotHitAnimation();
    }

    protected override void GetKnockBack(GameObject source)
    {
        //Do nothing
        return;
    }

    public override void Die()
    {
        base.Die();
        if (onResourceDeathCallback != null)
        {
            onResourceDeathCallback();
        }
    }

    private void PlayGotHitAnimation()
    {
        if (gotHitAnimationTimer.Value > 0)
        {
            if (IsServer)
                gotHitAnimationTimer.Value -= Time.deltaTime;
            //Debug.Log("Evaluated value: " + SetupSingleton.Instance.gotHitAnimationCurve.Evaluate(Mathf.Max(0, gotHitAnimationTimer) / gotHitAnimationMaxTime));
            objectGraphic.transform.localScale = Vector3.Lerp(Vector3.one, minScale, SetupSingleton.Instance.gotHitAnimationCurve.Evaluate(Mathf.Max(0, gotHitAnimationTimer.Value) / gotHitAnimationMaxTime));
        }
    }

    private void TriggerGotHitAnimation(GameObject dmgSource)
    {
        Debug.Log("Trigger got hit animation");
        gotHitAnimationTimer.Value = gotHitAnimationMaxTime;
    }
}
