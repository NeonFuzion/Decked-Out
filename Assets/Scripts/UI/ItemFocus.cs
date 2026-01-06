using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemFocus : MonoBehaviour
{
    [SerializeField] Vector2 pixelOffset;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI focusName, focusType, focusAmount, focusMainStats, focusSubStats, focusDescription, focusAbility;

    Vector2 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.AddOnUnfocusItemListener(() => gameObject.SetActive(false));

        offset = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = MainCamera.MousePixelPosition() - offset + pixelOffset;
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
        focusType.SetText("Weapon");
        focusMainStats.gameObject.SetActive(true);
        focusMainStats.SetText($"Attack - {weapon.Attack}");

        if (weapon.Substat.Stat == PlayerStat.None) return;
        focusSubStats.gameObject.SetActive(true);
        focusSubStats.SetText(weapon.Substat.ToString());

        focusAbility.SetText("");
    }

    void ProcessAccessory(Accessory accessory)
    {
        if (!accessory) return;
        focusType.SetText("Accessory");
        focusAbility.gameObject.SetActive(true);
        focusAbility.SetText($"<i>{accessory.PassiveEffectSO.Name}</i>\n{accessory.PassiveEffectSO.Description}");
    }

    void ProcessArmor(Armor armor)
    {
        if (!armor) return;
        focusType.SetText("Armor");
        focusMainStats.gameObject.SetActive(true);
        focusMainStats.SetText($"Defense - {armor.Defense}");

        if (armor.SecondaryStat.Stat != PlayerStat.None)
        {
            focusMainStats.text += "\n" + armor.SecondaryStat;
        }

        if (armor.Substats.Length > 0)
        {
            focusSubStats.gameObject.SetActive(true);
            focusSubStats.SetText(string.Join("\n", armor.Substats.Select(stat => stat.ToString())));
        }
    }

    public void DisplayItemStats(Item item, int amount = 1)
    {
        focusName.SetText(item.ItemName);
        focusType.SetText("Item");
        focusDescription.SetText(item.Description);

        if (amount > 1) focusAmount.SetText($"Owned: {amount}");
        else focusAmount.gameObject.SetActive(false);

        focusMainStats.gameObject.SetActive(false);
        focusSubStats.gameObject.SetActive(false);
        focusAbility.gameObject.SetActive(false);

        ProcessWeapon(item as Weapon);
        ProcessAccessory(item as Accessory);
        ProcessArmor(item as Armor);

        ResizeTooltip();
    }
}
