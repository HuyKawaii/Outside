using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowController : CharacterControl
{
    private float arrowSpeed = 20.0f;
    private Vector3 target;
    private Vector2 horizontalDirection;
    Vector3 arrowVelocity;
    float playerHeightOffset = 1.0f;
    float rotateSpeed = 2.0f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        CalculateInitialVelocity();
    }

    protected override void Update()
    {
        base.Update();
        arrowVelocity = (arrowSpeed * new Vector3(horizontalDirection.x, 0, horizontalDirection.y) + Vector3.down * yVelocity) * Time.deltaTime;
        RotateArrow();
        controller.Move(arrowVelocity);
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target + Vector3.up * playerHeightOffset;
        Debug.Log("Set target");
    }

    void RotateArrow()
    {
        Quaternion rotateDirection = Quaternion.LookRotation(arrowVelocity.normalized);
        transform.rotation = rotateDirection;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, rotateSpeed);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Player")
        {
            Debug.Log("Hit " + hit.transform.name);
            CharacterStats target = hit.transform.GetComponentInParent<CharacterStats>();
            characterCombat.DealDamage(target);
        }
        Destroy(gameObject);
    }

    protected override void GroundCheck()
    {
        isGround = false;
    }

    private void CalculateInitialVelocity()
    {
        Vector3 distance = target - transform.position;
        Vector3 horizontalDistance = new Vector2(distance.x, distance.z);
        horizontalDirection = horizontalDistance.normalized;
        float movingTime = horizontalDistance.magnitude / arrowSpeed;
        Vector3 verticalDirection = new Vector3(0, distance.y, 0);
        yVelocity = -(movingTime * gravity / 2 - verticalDirection.magnitude / movingTime);
        //Debug.Log("y velocity: " + yVelocity);
    }
}
