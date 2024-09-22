using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    PlayerStats playerStats;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = playerStats.health/playerStats.statData.maxHealth.GetValue();
    }
}
