using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] RectTransform highlight, hotbarParent, skillBarParent;
    [SerializeField] Image manaBar;
    [SerializeField] Sprite[] elementIcons;

    HotbarSlot[] hotbarSlots;
    SkillSlot[] skillBarSlots;

    void Awake()
    {
        hotbarSlots = hotbarParent.GetComponentsInChildren<HotbarSlot>();
        skillBarSlots = skillBarParent.GetComponentsInChildren<SkillSlot>();

        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    void Start()
    {
        
    }

    public void UpdateHotbarIndex(int index)
    {
        int tempIndex = Mathf.Clamp(index, 0, 3);
        HotbarSlot slot = hotbarSlots[tempIndex];
        highlight.position = slot.GetComponent<RectTransform>().position;
    }

    public void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            ItemStack itemStack = inventory.GetHotbarItemAtIndex(i);
            ConsumablesSO consumable = itemStack?.Item.ItemSO as ConsumablesSO;
            if (consumable) hotbarSlots[i].Initialize(consumable.Sprite, consumable.Cooldown, itemStack.Amount);
            else hotbarSlots[i].Initialize(null, 0, 0);
        }

        for (int i = 0; i < skillBarSlots.Length; i++)
        {
            Equipment equipInst = inventory.GetEquipmentAtIndex(4 + i);
            SkillTomeSO skillTome = equipInst?.EquipmentSO as SkillTomeSO;

            if (skillTome) skillBarSlots[i].InitializeSkill(skillTome, GetElementIcon(skillTome.Element));
            else skillBarSlots[i].InitializeSkill(null, null);
            
        }
    }

    public void UseHotbarItem(int index)
    {
        hotbarSlots[index].IncrementAmount();
        hotbarSlots[index].StartCooldown();
    }

    public void UpdateManaBar(float amount)
    {
        manaBar.fillAmount = amount;
    }

    public void TriggerSkillCooldown(int index)
    {
        skillBarSlots[index].StartCooldown();
    }

    Sprite GetElementIcon(Element element)
    {
        int index = (int)element;
        if (elementIcons == null || index >= elementIcons.Length) return null;
        return elementIcons[index];
    }
}
