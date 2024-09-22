using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    PlayerController playerController;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        playerController = PlayerManager.Instance.player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = playerController.stamina / playerController.maxStamina;
    }
}
