using UnityEngine;

public class SkillManager : MonoBehaviour
{
    SkillTomeSO skill;

    int skillComboIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSkill(SkillTomeSO skill)
    {
        this.skill = skill;

        skillComboIndex = 0;
    }
}
