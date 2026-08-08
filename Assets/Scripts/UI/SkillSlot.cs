using TMPro;
using UnityEngine;

public class SkillSlot : HotbarSlot
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject nameTextParent;

    public void InitializeSkill(SkillTomeSO skillTome, Sprite icon)
    {
        if (skillTome) Initialize(icon, skillTome.Cooldown, 0);
        else Initialize(null, 0, 0);

        bool hasSkill = skillTome != null;
        if (nameTextParent) nameTextParent.SetActive(hasSkill);
        if (nameText) nameText.SetText(hasSkill ? skillTome.ItemName : "");
    }
}
