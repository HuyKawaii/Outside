using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterControl : NetworkBehaviour
{
    protected CharacterController controller;
    protected CharacterCombat characterCombat;
    protected Transform groundCheck;

    protected bool isGround;
    protected float yVelocity = 0f;
    public float gravity = 9.81f;
    private float groundDistanceCheck = 0.2f;
    float maxFallingSpeed = 30.0f;
    public LayerMask groundLayer;

    public float walkingSpeed = 6.0f;
    public float runningSpeed = 10.0f;

    private Vector3 lastPosition;
    public Vector3 velocity;

    protected bool isKnockBack = false;
    protected bool knockBackTrigger = false;
    protected Vector3 knockBackVelocity = Vector3.zero;
    protected float knockUpVelocity;

    protected bool isAttacking;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        characterCombat = GetComponent<CharacterCombat>();
        groundCheck = transform.Find("GroundCheck");
        groundLayer = LayerMask.GetMask("Ground");
    }

    protected virtual void Start()
    {
        lastPosition = controller.transform.position;
    }
    protected virtual void Update()
    {
        if (!IsOwner)
            return;
     
        //Get velocity
        velocity = (controller.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = controller.transform.position;

        if (isKnockBack)
        {
            TriggerKnockBack();
            controller.Move(new Vector3(knockBackVelocity.x, 0, knockBackVelocity.z) * Time.deltaTime + Vector3.down * yVelocity * Time.deltaTime);
            //Debug.Log(gameObject.name + " is being Knockback at " + knockBackVelocity);
        }

        //Falling logic
        GroundCheck();

        if (isGround)
        {
            yVelocity = 2.0f;
            isKnockBack = false;
        }
        else
        {
            if (yVelocity >= maxFallingSpeed)
                yVelocity = maxFallingSpeed;
            else
                yVelocity += gravity * Time.deltaTime;
        }
    }

    protected virtual void GroundCheck()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistanceCheck, groundLayer) && (yVelocity > 0);
    }

    protected void TriggerKnockBack()
    {
        if (knockBackTrigger)
        {
            yVelocity = knockUpVelocity;
            knockBackTrigger = false;
            //Debug.Log("Trigger knockback");
        }
    }

    public void KnockBack(Vector3 knockBackDirection, float knockBackDistance, float knockBackTime)
    {
        isKnockBack = true;
        knockBackTrigger = true;
        knockBackVelocity = knockBackDirection * knockBackDistance / knockBackTime;
        knockUpVelocity = knockBackTime * -0.5f * 9.8f;
    }
    public void StartAttack() { isAttacking = true; }

    public void StopAttack() { isAttacking = false; }
}
