using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI amountText;

    int index;
    bool isEquipment, isEquiped;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnFocus();
        if (eventData.button == PointerEventData.InputButton.Right) OnEquip();
    }

    public void Initialize(Sprite sprite, int amount, int index, bool isEquipment, bool isEquiped)
    {
        this.index = index;
        this.isEquipment = isEquipment;
        this.isEquiped = isEquiped;
        image.sprite = sprite;

        if (isEquipment || amount == 1) return;
        amountText.text = amount.ToString();
    }

    public void OnFocus()
    {
        EventManager.InvokeOnFocusItem(index, isEquiped, transform);
    }

    public void OnEquip()
    {
        if (!isEquipment) return;
        if (isEquiped && index == 0) return;

        EventManager.InvokeOnEquip(index, isEquiped);
        isEquiped = !isEquiped;
    }
}
