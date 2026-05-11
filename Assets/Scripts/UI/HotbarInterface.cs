using System.Linq;
using UnityEngine;

public class HotbarInterface : MonoBehaviour
{
    [SerializeField] RectTransform highlight;
    [SerializeField] HotbarSlot[] hotbarSlots;
    //[SerializeField] 

    void Awake()
    {
        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHotbarIndex(int index)
    {
        int tempIndex = Mathf.Max(index - 1, 0);
        HotbarSlot slot = hotbarSlots[tempIndex];

        if (slot.IsEmpty) return;
        highlight.anchoredPosition = slot.GetComponent<RectTransform>().anchoredPosition;
    }

    public void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            ConsumablesSO consumable = inventory.GetEquipment(i) as ConsumablesSO;
            hotbarSlots[i].Initialize(consumable.Sprite, consumable.Cooldown);
        }
    }
}
