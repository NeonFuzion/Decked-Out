using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] UnityEvent<Weapon> onSetWeaponAsMainHand;

    MainHand mainHand;
    MainHand[] hotbar;

    int hotbarIndex;

    void Awake()
    {
        hotbarIndex = 0;
        hotbar = new MainHand[4];
        EventManager.AddOnInventoryUpdatedListener(UpdateHotbar);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHotbar()
    {
        Inventory inventory = Inventory.Instance;
        for (int i = 0; i < 4; i++)
        {
            hotbar[i] = inventory.GetEquipment(i) as MainHand;
        }
        UpdateHotbarIndex(hotbarIndex);
    }

    public void UpdateHotbarIndex(int index)
    {
        int tempIndex = Mathf.Max(index - 1, 0);
        if (!hotbar[tempIndex]) return;
        hotbarIndex = tempIndex;
        mainHand = hotbar[hotbarIndex];

        Weapon weapon = mainHand as Weapon;

        if (weapon)
        {
            onSetWeaponAsMainHand?.Invoke(weapon);
        }
    }
}
