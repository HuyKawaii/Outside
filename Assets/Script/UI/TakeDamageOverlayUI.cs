using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamageOverlayUI : MonoBehaviour
{
    [SerializeField] private Image takeDamageOverlay;
    private float timer;
    private float maxTimer = 1.5f;

    private void Start()
    {
        PlayerManager.Instance.player.GetComponent<PlayerStats>().takingDamageCallBack += DisplayTakeDamageOverlay;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            takeDamageOverlay.color = new Color(1, 1, 1, Mathf.Max(0, timer / maxTimer));
        }
    }

    public void DisplayTakeDamageOverlay(GameObject source)
    {
        timer = maxTimer;
    }
}
