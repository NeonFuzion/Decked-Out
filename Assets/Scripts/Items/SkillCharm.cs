using UnityEngine;

public class SkillCharm : ScriptableObject
{
    protected GameObject player;

    public virtual void Activate()
    {

    }

    public virtual void UpdateState(float increment)
    {

    }

    public virtual void Initialize(GameObject player)
    {
        this.player = player;
    }
}

public enum CharmType { Skill, Ultimate }