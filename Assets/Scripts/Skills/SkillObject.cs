using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Initialize(SkillTomeSO skillTomeSO, HotbarManager hotbarManager);

    public abstract void ActivateSkill(InputActionPhase inputPhase);
}
