using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private RectTransform loadingScreen;

    void Start()
    {
        GameManager.Instance.gameLoadComplete += CloseLoadingScreen;
    }

    private void OnDestroy()
    {
        GameManager.Instance.gameLoadComplete -= CloseLoadingScreen;
    }

    private void CloseLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }
}
