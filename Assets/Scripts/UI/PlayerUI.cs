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
        hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        skillBarSlots = GetComponentsInChildren<SkillSlot>();

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
            EquipmentInstance equipInst = inventory.GetEquipment(i);
            ConsumablesSO consumable = equipInst?.EquipmentData as ConsumablesSO;
            hotbarSlots[i].Initialize(consumable ? consumable.Sprite : null, consumable ? consumable.Cooldown : 0);
        }

        for (int i = 0; i < skillBarSlots.Length; i++)
        {
            EquipmentInstance equipInst = inventory.GetEquipment(8 + i);
            SkillTomeSO skillTome = equipInst?.EquipmentData as SkillTomeSO;
            Sprite icon = skillTome != null ? GetElementIcon(skillTome.Element) : null;
            skillBarSlots[i].InitializeSkill(skillTome, icon);
        }
    }

    public void UpdateManaBar(float amount)
    {
        manaBar.fillAmount = amount;
    }

    public void TriggerSkillCooldown(int index)
    {
        if (index < 0 || index >= skillBarSlots.Length) return;
        skillBarSlots[index].StartCooldown();
    }

    Sprite GetElementIcon(Element element)
    {
        int index = (int)element;
        if (elementIcons == null || index >= elementIcons.Length) return null;
        return elementIcons[index];
    }
}
