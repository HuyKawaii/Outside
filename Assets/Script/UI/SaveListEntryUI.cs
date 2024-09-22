using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveListEntryUI : MonoBehaviour
{
    public int index {get; private set;}
    private Color selectedColor = new Color32(99, 53, 11, 255);
    private Color unselectedColor = new Color32(135, 73, 17, 255);

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetName(string name)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    public void NewEntryGotSelected(int newEntryIndex)
    {
        if (index != newEntryIndex)
            SetEntryColor(unselectedColor);
        else
            SetEntryColor(selectedColor);
    }

    private void SetEntryColor(Color color)
    {
        transform.GetComponent<Image>().color = color;
    }
}
