using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewWorldMenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform createNewWorldPanel;
    [SerializeField] private Button closeNewWorldMenuButton;
    [SerializeField] private Button createNewWorldButton;
    [SerializeField] private TMP_InputField worldNameInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField worldSeedInputField;

    private void Start()
    {
        closeNewWorldMenuButton.onClick.AddListener(() =>
        {
            createNewWorldPanel.gameObject.SetActive(false);
        });

        createNewWorldButton.onClick.AddListener(() =>
        {
            if (worldNameInputField.text == "")
                return;
            if (playerNameInputField.text == "")
                return;
           
            if (!SaveSelectionManager.Instance.CheckWorldName(worldNameInputField.text))
            {
                GameMenuUIManager.Instance.DisplayErrorMessage("World name existed. Choose a different name");
                return;
            }

            GameMenuUIManager.Instance.CloseCreateWorldMenu();
            GameMenuUIManager.Instance.DisplayLoadingScreen();

            GameManager.Instance.CreateNewGame(worldNameInputField.text, worldSeedInputField.text == "" ? -1 : int.Parse(worldSeedInputField.text), playerNameInputField.text);
        });
    }

}
