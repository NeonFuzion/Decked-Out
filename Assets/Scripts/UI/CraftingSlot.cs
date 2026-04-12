using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CraftingSlot : Slot, IPointerClickHandler
{
    [SerializeField] Color uncraftableColor, baseColor;
    [SerializeField] UnityEvent<CraftingData> onClick;

    CraftingData craftingData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void FocusOnItem()
    {
        if (craftingData == null) return;
        EventManager.InvokeOnFocusItem(new (craftingData.CraftingRecipe.Output, 0));
    }

    public void Initialize(CraftingData craftingData, UnityAction<CraftingData> unityAction)
    {
        this.craftingData = craftingData;

        backgroundImage.color = craftingData.IsCraftable ? baseColor : uncraftableColor;
        UpdateItem(craftingData.CraftingRecipe.Output.Sprite, 1);
        onClick?.AddListener(unityAction);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(craftingData);
    }
}
