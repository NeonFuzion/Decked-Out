using UnityEngine;

public class SkillCharm : Equipment
{
    protected GameObject player;
    protected CharmType charmType;

    public CharmType CharmType { get => charmType; }

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