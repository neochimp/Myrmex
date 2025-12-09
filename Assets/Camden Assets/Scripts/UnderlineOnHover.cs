using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnderlineOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject underline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        underline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        underline.SetActive(false);
    }
}
