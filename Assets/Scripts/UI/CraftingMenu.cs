using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    [SerializeField] CraftingRecipes craftingRecipes;
    [SerializeField] ItemFocus itemFocus;
    [SerializeField] Transform materialsParent, recipeParent;
    [SerializeField] GameObject prefabQuotaSlot, prefabItemSlot;

    Inventory inventory;

    void Awake()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
        EventManager.AddOnInventoryUpdatedListener(UpdateCraftingMenu);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = Inventory.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FocusOnItem(int index, bool isEquiped, Transform itemSlot)
    {
        int itemAmount = 1;
        Item item;
        if (isEquiped)
        {
            item = inventory.GetEquipment(index);
        }
        else
        {
            if (inventory.GetItem(index) == null) return;
            ItemStack slot = inventory.GetItem(index);
            item = slot.Item;
            itemAmount = slot.Amount;
        }

        itemFocus.DisplayItemStats(item, itemAmount);
    }

    void UpdateCraftingMenu()
    {
        foreach (Transform material in materialsParent) material.gameObject.SetActive(false);
        
        List<Item> materials = new ();
        for (int i = 0; i < inventory.GetItemCount(); i++)
        {
            Item inventoryItem = inventory.GetItem(i).Item;
            if (materials.Contains(inventoryItem)) continue;
            materials.Add(inventoryItem);
        }

        List<CraftingRecipe> visibleRecipes = new ();
        List<CraftingRecipe> allRecipes = craftingRecipes.Recipes.ToList();
        materials.ForEach(material =>
        {
            allRecipes.ForEach(recipe =>
            {
                if (!recipe.Ingredients.Contains(material)) return;
                visibleRecipes.Add(recipe);
            });
        });
    }
}
