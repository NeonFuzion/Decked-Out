using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DungeonEntrance : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int sceneIndex;

    void OnLeftClick()
    {   
        SceneHandler.SetScene(2);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick();
    }
}
