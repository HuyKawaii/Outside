using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWindowUI : MonoBehaviour
{
    private static ConfirmationWindowUI Instance;
    private System.Action storedOnConfirmAction;
    [SerializeField] private TextMeshProUGUI confirmDialog;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }

    }
    private void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            if (storedOnConfirmAction != null)
                storedOnConfirmAction();
            storedOnConfirmAction = null;
            Instance.gameObject.SetActive(false);
        });

        cancelButton.onClick.AddListener(() =>
        {
            storedOnConfirmAction = null;
            Instance.gameObject.SetActive(false);
        });

        closeButton.onClick.AddListener(() =>
        {
            storedOnConfirmAction = null;
            Instance.gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    public static void OpenConfirmationWindow(string message, System.Action onConfirmAction)
    {
        Instance.confirmDialog.text = message;
        Instance.storedOnConfirmAction = onConfirmAction;
        Instance.gameObject.SetActive(true);
    }

}
