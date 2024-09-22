using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private TextMeshPro playerNameText;
    Transform targetTransform;
    private PlayerController localPlayerController;

    private void Start()
    {
        playerNameText = GetComponent<TextMeshPro>();
        playerNameText.text = playerName.Value.ToString();
        playerName.OnValueChanged += (FixedString32Bytes oldValue, FixedString32Bytes newValue) =>
        {
            playerNameText.text = newValue.ToString();
        };

        SetPlayerName();
        GameManager.Instance.gameLoadComplete += SetTarget;
    }
    void Update()
    {
        if (localPlayerController?.isGameStarted == true)
            ShowText();
    }

    public override void OnDestroy()
    {
        GameManager.Instance.gameLoadComplete -= SetTarget;
        base.OnDestroy();
    }

    private void ShowText()
    {
        Vector3 lookDirection = transform.position - targetTransform.position;
        Quaternion rotateDirection = Quaternion.LookRotation(lookDirection);
        transform.rotation = rotateDirection;
    }

    public string GetPlayerName()
    {
        return playerName.Value.ToString();
    }

    private void SetTarget()
    {
        targetTransform = PlayerManager.Instance.player.transform;
    }

    private void SetPlayerName()
    {
        if (IsOwner)
            playerName.Value = GameManager.Instance.playerName;
    }
}
