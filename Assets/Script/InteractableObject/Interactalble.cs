using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Interactalble : NetworkBehaviour
{
    public bool isLookedAt = false;
    protected string hoverText;
    protected GameObject text;
    protected MeshRenderer textMesh;
    private Transform targetTransform;
    private float raycastDistance = 100f;
    protected virtual void Awake()
    {
        text = transform.GetChild(0).gameObject;
        SetHoverText();
        text.GetComponent<TextMesh>().text = hoverText;
        textMesh = text.GetComponent<MeshRenderer>();
    }

    protected virtual void Start()
    {
        SnapToGround();
        targetTransform = PlayerManager.Instance.player.transform.Find("PlayerCamera").transform;
    }

    protected virtual void Update()
    {
        if (isLookedAt)
            ShowText();
        else
            textMesh.enabled = false;

        isLookedAt = false;
    }

    protected virtual void SnapToGround()
    {
        //Debug.Log("Try to snap");
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, raycastDistance, ~SetupSingleton.Instance.groundLayer))
        {
            gameObject.transform.localPosition = hit.point + Vector3.up * 0.2f;
            //Debug.Log(gameObject.name + " snapped to layer: " + hit.collider.gameObject.layer);
        }
    }

    protected abstract void SetHoverText();
    protected virtual void ShowText()
    {
        textMesh.enabled = true;
        Vector3 lookDirection = transform.position - targetTransform.position;
        Quaternion rotateDirection = Quaternion.LookRotation(lookDirection);
        text.transform.rotation = rotateDirection;
    }

    protected void DisableHoverText()
    {
        text.SetActive(false);
    }

    public abstract void Interact(Transform player);
}
