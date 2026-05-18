using TMPro;
using UnityEngine;

public class SkillSlot : HotbarSlot
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject nameTextParent;

    public void InitializeSkill(SkillTomeSO skillTome, Sprite icon)
    {
        Initialize(icon, skillTome != null ? skillTome.Cooldown : 0);

        bool hasSkill = skillTome != null;
        if (nameTextParent) nameTextParent.SetActive(hasSkill);
        if (nameText) nameText.SetText(hasSkill ? skillTome.ItemName : "");
    }
}
