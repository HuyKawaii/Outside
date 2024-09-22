using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : CharacterControl
{
    //Player compoennets
    [SerializeField] Transform playerGraphic;
    [SerializeField] Transform playerCamera;
    public bool isGameStarted;
    //Moving variables
    private Vector3 move;
    private float moveX, moveY;
    public float speed;

    private float jumpHeight = 1.0f;
    private float dashTimer = 0.2f;
    private bool isDashing = false;
    private float dashingSpeed = 40.0f;
    private bool isRunning = false;

    //Stamina variables
    public float maxStamina
    {  get; private set; }
    public float stamina
    {  get; private set; }
    public float maxHunger
    { get; private set; }
    public float hunger
    { get; private set; }

    private float hungerSpeed = 0.33f;
    private float staminaRecover = 10.0f;
    private float runningStamina = 10.0f;
    private float dashingStamina = 30.0f;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        BeforeGameStartInitialization();
        if (!IsOwner)
        {
            DisableNonOwnerComponent();
            return;
        }

        base.Start();
        speed = walkingSpeed;
        maxStamina = characterCombat.myStats.statData.maxStamina.GetValue();
        maxHunger = characterCombat.myStats.statData.maxHunger.GetValue();
        stamina = maxStamina;
        hunger = maxHunger;
    }

    protected override void Update()
    {
        if (!IsOwner) return;
        if (((PlayerCombat)characterCombat).isDead || !isGameStarted) return;
        base.Update();

        HandleMovement();

        if (transform.position.y < -50)
            transform.position = new Vector3(transform.position.x, 20, transform.position.z);

    }

    private void HandleMovement()
    {
        //Move
        if (!isDashing)
            move = Vector3.zero;

        //Get Horizontal input
        if (!InventoryUI.instance.isInventoryOpen && !isDashing)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");

            move = transform.right * moveX + transform.forward * moveY;

            if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
            {
                speed = runningSpeed;
                isRunning = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || stamina <= 0)
            {
                speed = walkingSpeed;
                isRunning = false;
            }
            if (Input.GetKeyDown(KeyCode.R) && AbilityManager.Instance.abilityUnlock[(int)Ability.Dash] && stamina >= dashingStamina)
                Dash();
        }

        Vector3.Normalize(move);

        //Fall and Jump
        if (isGround && Input.GetKey(KeyCode.Space))
            yVelocity = -Mathf.Sqrt(2 * gravity * jumpHeight);

        if (isDashing)
        {
            yVelocity = 0;
            if (move == Vector3.zero)
                move = transform.forward;
            controller.Move(move * Time.deltaTime * dashingSpeed);
            //Debug.Log("Dashing in direction " + move);
        }

        //Knockback check
        if (!isKnockBack && !isDashing)
            controller.Move(move * Time.deltaTime * speed + Vector3.down * yVelocity * Time.deltaTime);

        if (!isRunning && !isDashing)
        {
            //Debug.Log("Reconvering stamina");
            if (stamina < maxStamina)
            {
                if (hunger > 0)
                {
                    stamina += staminaRecover * Time.deltaTime;
                    hunger -= hungerSpeed * Time.deltaTime * 4;
                }
                
            }
            else
                stamina = maxStamina;
        }
        else if (isRunning)
            stamina -= runningStamina * Time.deltaTime;

        if (hunger > 0)
        {
            hunger -= hungerSpeed * Time.deltaTime;
            //Debug.Log("Hunger: " + hunger);
        }
        else
            hunger = 0;
    }

    private void Dash()
    {
        //Debug.Log("Player dashes");
        isDashing = true;
        stamina -= dashingStamina;
        StartCoroutine(DashCoutDownCouroutine()); 
    }

    private IEnumerator DashCoutDownCouroutine()
    {
        yield return new WaitForSeconds(dashTimer);
        isDashing = false;
    }

    private void DisableNonOwnerComponent()
    {
        foreach (Transform child in playerGraphic)
            child.gameObject.layer = 0;

        for (int i = 2; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }

    private void BeforeGameStartInitialization()
    {
        playerCamera.gameObject.SetActive(false);
        //Debug.Log("Player init");
        isGameStarted = false;
    }

    public void GameStartInitialization()
    {
        playerCamera.gameObject.SetActive(true);
        //transform.position = new Vector3(0, 100.0f, 0);
        //Debug.Log("Set player position");
    }

    public void RestoreHunger(float nutrientAmount)
    {
        hunger += nutrientAmount;
        if (hunger > maxHunger)
            hunger = maxHunger;
    }

    [Command]
    public void SetWalkingSpeed(float newWalkingSpeed)
    {
        walkingSpeed = newWalkingSpeed;
    }
}
