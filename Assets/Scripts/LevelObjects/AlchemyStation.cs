using UnityEngine;

public class AlchemyStation : RoomObject
{
    CraftingMenu craftingMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadCraftingMenu()
    {
        UIManager.Instance.OpenMenu(craftingMenu.gameObject);
        craftingMenu.Initialize();
    }

    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        return new(true);
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        craftingMenu = FindAnyObjectByType<CraftingMenu>(FindObjectsInactive.Include);
    }
}
