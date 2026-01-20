using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButtonInfoHandler : MonoBehaviour
{
    [SerializeField]
    GameObject canvasObject;
    [SerializeField]
    TextMeshProUGUI infoText;
    [SerializeField]
    Color defaultColor = Color.green;
    [SerializeField]
    Color warningColor = Color.yellow;
    [SerializeField]
    Color errorColor = Color.red;

    Coroutine hideCoroutine;

    [Header("UI Offset")]
    [SerializeField]
    RectTransform uiObjectRect;
    [SerializeField]
    Vector2 uiOffset = Vector2.zero;
    [SerializeField]
    Vector2 uiSizeOffset = Vector2.zero;

    private Vector2 startUIPosition;
    private Vector2 startUISize;

    private void Awake()
    {
        startUIPosition = uiObjectRect.anchoredPosition;
        startUISize = uiObjectRect.sizeDelta;
    }
    private void OnEnable()
    {
        uiObjectRect.anchoredPosition = startUIPosition + uiOffset;
        uiObjectRect.sizeDelta = startUISize + uiSizeOffset;
    }

    public void ShowInfoUI(string text, InfoType infoType = InfoType.Success)
    {
        infoText.text = text;

        canvasObject.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideInfoUI());

        SetTextColor(infoType);
    }

    IEnumerator HideInfoUI()
    {
        yield return new WaitForSeconds(3f);

        canvasObject.SetActive(false);

        hideCoroutine = null;
    }

    private void SetTextColor(InfoType infoType)
    {
        switch (infoType)
        {
            case InfoType.Success:
                infoText.color = defaultColor;
                break;

            case InfoType.Warning:
                infoText.color = warningColor;
                break;

            case InfoType.Error:
                infoText.color = errorColor;
                break;
        }
    }
}
