using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UISoundManager.Instance.PlaySound(UISoundManager.UISoundType.Hover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UISoundManager.Instance.PlaySound(UISoundManager.UISoundType.Click);
    }
}
