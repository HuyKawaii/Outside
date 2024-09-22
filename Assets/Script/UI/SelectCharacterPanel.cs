using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterPanel : MonoBehaviour
{
    public SelectCharacterMenuUI selectCharacterMenuUI;
    private void OnEnable()
    {
        selectCharacterMenuUI.ResetUI();
    }
}
