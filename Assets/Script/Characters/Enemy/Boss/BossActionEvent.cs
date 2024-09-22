using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActionEvent : MonoBehaviour
{
    BossController controller;
    private void Awake()
    {
        controller = transform.parent.GetComponent<BossController>();
    }

    public void DashEvent()
    {
        controller.BossDash(controller.target.position);
        Debug.Log("Target: " + controller.target.position);
    }

    public void EndDash()
    {
        controller.StopDash();
    }
}
