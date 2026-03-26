using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CraftingSlot : Slot, IPointerClickHandler
{
    [SerializeField] Color uncraftableColor;

    Color baseColor;
    CraftingData craftingData;
    UnityEvent<CraftingData> onClick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseColor = backgroundImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(CraftingData craftingData, UnityAction<CraftingData> unityAction)
    {
        this.craftingData = craftingData;

        backgroundImage.color = craftingData.IsCraftable ? baseColor : uncraftableColor;
        onClick?.AddListener(unityAction);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(craftingData);
    }
}
