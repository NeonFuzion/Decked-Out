using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragAndDrop : MonoBehaviour, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    [SerializeField] UnityEvent onStartDrag, onEndDrag;

    int index;
    SlotType slotType;

    Image image;
    Transform postDragParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        postDragParent = transform.parent;
        transform.SetParent(transform.root);
        //onStartDrag?.Invoke();
        EventManager.InvokeOnPickupItem(index, slotType);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = MainCamera.MouseWorldPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(postDragParent);
        //onEndDrag?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();

        EventManager.AddOnDropItemListener((int index, SlotType slotType) =>
        {
            if (image.raycastTarget) return;
            OnEndDrag(null);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int index, SlotType slotType)
    {
        this.index = index;
        this.slotType = slotType;
    }
}
