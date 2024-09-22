using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ToolTip : MonoBehaviour
{
    private RectTransform tipWindow;
    private TextMeshProUGUI tipText;
    private int horizontalPadding = 5;
    private int vertiacalPadding = 5;
    const int MAX_WIDTH = 200;
    private void Awake()
    {
        tipWindow = this.GetComponent<RectTransform>();
        tipText = GetComponentInChildren<TextMeshProUGUI>(true);
        tipText.text = default;
        this.gameObject.SetActive(false);
    }

    public void SetText(string newText)
    {
        tipText.text = newText;
    }

    public void ShowTooltip(Vector3 position, string text)
    {
        this.gameObject.SetActive(true);
        SetText(text);
        transform.position = position;
        tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > MAX_WIDTH ? MAX_WIDTH : tipText.preferredWidth + horizontalPadding, tipText.preferredHeight + vertiacalPadding);
        //tipWindow.transform.position = transform.parent.GetComponent<RectTransform>().transform.position + new Vector3(tipWindow.sizeDelta.x, tipWindow.sizeDelta.y);
    }

    public void HideTooltip()
    {
        tipText.text = default;
        this.gameObject.SetActive(false);
    }
}
