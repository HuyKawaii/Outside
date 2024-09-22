using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounterUI : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    PlayerEconManager playerEconManager;
    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        playerEconManager = PlayerManager.Instance.player.GetComponent<PlayerEconManager>();
    }

    void Update()
    {
        textMeshProUGUI.text = playerEconManager.GetGoldAmount().ToString();
    }
}
