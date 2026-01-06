using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] UnityEvent onClick, onEnter, onExit;

    bool isMouseOver;

    public UnityEvent OnClick { get => onClick; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        onExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isMouseOver) return;
        onClick?.Invoke();
    }
}
