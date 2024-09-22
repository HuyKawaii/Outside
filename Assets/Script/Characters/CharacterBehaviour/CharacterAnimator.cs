using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterAnimator : NetworkBehaviour
{
    private float locomotionSmooth = 0.1f;
    CharacterControl characterControl;
    protected Animator characterAnimator;

    protected virtual void Start()
    {
        characterControl = GetComponent<CharacterControl>();
        characterAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner)
            return;
        float speedPercent = UtilFunc.GetHorizontalSpeed(characterControl.velocity) / characterControl.runningSpeed;
        characterAnimator.SetFloat("speedPercent", speedPercent);
    }
}