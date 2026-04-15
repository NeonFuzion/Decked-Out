using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemFocus : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI focusName, focusType, focusAmount, focusMainStats, focusSubStats, focusDescription, focusAbility;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform.GetComponent<Image>().raycastTarget = false;

        EventManager.AddOnFocusItemListener(DisplayItemStats);
        EventManager.AddOnUnfocusItemListener(() => rectTransform.gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.position = MainCamera.MouseWorldPosition();
    }

    void ResizeTooltip()
    {
        VerticalLayoutGroup verticalLayoutGroup = rectTransform.GetComponent<VerticalLayoutGroup>();
        
        RectOffset padding = verticalLayoutGroup.padding;
        float height = padding.bottom + padding.top;
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            RectTransform child = (RectTransform)rectTransform.GetChild(i);

            if (!child.gameObject.activeInHierarchy) continue;
            TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();
            if (!textMeshPro) textMeshPro = child.GetChild(0).GetComponent<TextMeshProUGUI>();
            textMeshPro.ForceMeshUpdate();

            float childHeight = textMeshPro.GetRenderedValues(false).y;
            height += childHeight;
            child.sizeDelta = new (child.sizeDelta.x, childHeight);
            
            if (i == 0) continue;
            height += verticalLayoutGroup.spacing;
        }

        rectTransform.sizeDelta = new (rectTransform.rect.width, height);
    }

    void ProcessWeapon(Weapon weapon)
    {
        if (!weapon) return;
        int maxDamage = weapon.Animations.Max(animation => animation.Damage);
        int minDamage = weapon.Animations.Min(animation => animation.Damage);
        string text = maxDamage == minDamage ? maxDamage + "" : $"{minDamage} ~ {maxDamage}";

        focusType.SetText("Weapon");
        focusMainStats.gameObject.SetActive(true);
        focusMainStats.SetText($"Damage: {text}");

        focusAbility.SetText("");
    }

    void ProcessArmor(Armor armor)
    {
        if (!armor) return;
        focusType.SetText("Armor");
        focusMainStats.gameObject.SetActive(true);
        focusMainStats.SetText($"Defense: {armor.Defense}");

        if (armor.PassiveEffectSO)
        {
            focusAbility.gameObject.SetActive(true);
            focusAbility.SetText($"<i>{armor.PassiveEffectSO.Name}</i>\n{armor.PassiveEffectSO.Description}");
        }

        if (armor.Substats.Length > 0)
        {
            focusSubStats.gameObject.SetActive(true);
            focusSubStats.SetText(string.Join("\n", armor.Substats.Select(stat => stat.ToString())));
        }
    }

    void ProcessSkillTome(SkillTome skillTome)
    {
        if (!skillTome) return;
        int maxDamage = skillTome.DamageValues.Max();
        int minDamage = skillTome.DamageValues.Min();
        string text = maxDamage == minDamage ? maxDamage + "" : $"{minDamage} ~ {maxDamage}";

        focusMainStats.gameObject.SetActive(true);
        focusSubStats.gameObject.SetActive(true);

        focusType.SetText("Skill Tome");
        focusMainStats.SetText($"Damage: {text}");
        focusSubStats.SetText($"Cost: {skillTome.ResourceCost} {skillTome.CombatResource}<br>Cooldown: {skillTome.Cooldown} sec.");
    }

    public void DisplayItemStats(ItemStack itemStack)
    {
        int amount = itemStack.Amount;
        Item item = itemStack.Item;
        Update();
        rectTransform.gameObject.SetActive(true);
        
        focusName.SetText(item.ItemName);
        focusType.SetText("Item");
        focusDescription.SetText(item.Description);

        if (amount > 0) focusAmount.SetText($"Owned: {amount}");
        else focusAmount.gameObject.SetActive(false);

        focusMainStats.gameObject.SetActive(false);
        focusSubStats.gameObject.SetActive(false);
        focusAbility.gameObject.SetActive(false);

        ProcessWeapon(item as Weapon);
        ProcessArmor(item as Armor);
        ProcessSkillTome(item as SkillTome);
        // NOTE: Add main hand support

        ResizeTooltip();
    }
}
