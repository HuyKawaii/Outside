using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CharacterCombat
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float attackCooldown = 0;
    public bool isDead
    { get; private set; }
    public delegate void PlayerDieCallback();
    public PlayerDieCallback onPlayerDieCallback;
    protected float attackImpactDuration = 0.1f;
    protected float attackStartUpDuration = 0.5f;
    protected float attackRecoveryDuration = 1.5f;

    protected override void Start()
    {
        base.Start();
        ((PlayerStats)myStats).deadCallBack += PlayerDied;
        isDead = false;
    }

    protected override void Update()
    {
        if ((InventoryUI.instance?.isInventoryOpen ?? true) || isDead || !IsOwner) return; //|| !GameManager.Instance.isGameStarted) return;

        base.Update();
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
            InitiateAttack();
    }

    void InitiateAttack()
    {
        if (attackCooldown <= 0)
        {
            attackCooldown = 1 / myStats.statData.attackSpeed.GetValue();
            Attack();
        }
    }

    protected override void Attack()
    {
        base.Attack();
        animator.SetTrigger("onAttack");
        StartCoroutine(AttackCoroutine());
    }

    protected virtual IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackStartUpDuration);
        attackPos.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(attackImpactDuration);
        attackPos.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(attackRecoveryDuration);
        characterControl.StopAttack();
    }

    protected override void SetTargetTag()
    {
        base.SetTargetTag();
        targetTag = "Enemy";
    }

    public void PlayerDied()
    {
        isDead = true;
        if (onPlayerDieCallback != null)
            onPlayerDieCallback();
    }

    public void RevivePlayer()
    {
        isDead = false;
        myStats.Heal();
    }

    public void Heal(float healAmount)
    {
        myStats.Heal(healAmount);
    }
}
