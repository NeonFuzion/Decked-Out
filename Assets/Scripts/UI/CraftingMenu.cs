using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    [SerializeField] CraftingRecipes craftingRecipes;
    [SerializeField] ItemFocus itemFocus;
    [SerializeField] Transform materialsParent, recipeParent;
    [SerializeField] GameObject prefabQuotaSlot, prefabCraftingSlot;

    Inventory inventory;
    CraftingRecipe currentRecipe;

    void Awake()
    {
        EventManager.AddOnFocusItemListener(FocusOnItem);
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

    void SelectCraftingRecipe(CraftingData craftingData)
    {
        currentRecipe = craftingData.CraftingRecipe;

        for (int i = 0; i < currentRecipe.Ingredients.Length; i++)
        {
            GameObject materialChild = i >= materialsParent.childCount ? Instantiate(prefabQuotaSlot) : recipeParent.GetChild(i).gameObject;
            QuotaSlot quotaSlot = materialChild.GetComponent<QuotaSlot>();
            quotaSlot.Initialize(currentRecipe.Ingredients[i], craftingData.AvailableIngredients[i].Amount);
        }

        for (int i = currentRecipe.Ingredients.Length; i < materialsParent.childCount; i++)
        {
            materialsParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    void UpdateCraftingMenu()
    {
        if (!gameObject.activeInHierarchy) return;

        // Hide old recipes & ingredients
        foreach (Transform material in materialsParent) material.gameObject.SetActive(false);
        foreach (Transform recipe in recipeParent) recipeParent.gameObject.SetActive(false);
        
        // Find all materials in inventory
        List<ItemStack> materials = new ();
        for (int i = 0; i < inventory.GetItemCount(); i++)
        {
            ItemStack inventoryItem = inventory.GetItem(i);
            if (materials.Contains(inventoryItem)) continue;
            materials.Add(inventoryItem);
        }

        // Find all recipes that contain materials that exist in inventory even if they can't be fully crafted
        List<CraftingData> visibleRecipes = new ();
        List<CraftingRecipe> allRecipes = craftingRecipes.Recipes.ToList();
        allRecipes.ForEach(recipe =>
        {
            bool isCraftable = true;
            bool isMaterialFound = false;
            List<ItemStack> availableIngredients = new ();
            recipe.Ingredients.ToList().ForEach(ingredient =>
            {
                ItemStack inventoryStack = materials.Find(stack => stack.Item == ingredient.Item);
                if (inventoryStack != null)
                {
                    isMaterialFound = true;
                    availableIngredients.Add(inventoryStack);
                }
                else
                {
                    availableIngredients.Add(new (ingredient.Item, 0));
                }

                if (inventoryStack.Amount < ingredient.Amount)
                {
                    isCraftable = false;
                }
            });

            if (!isMaterialFound) return;
            visibleRecipes.Add(new (isCraftable, recipe, availableIngredients.ToArray()));
        });

        // Populate recipes
        for (int i = 0; i < visibleRecipes.Count; i++)
        {
            GameObject recipeChild = i >= recipeParent.childCount ? Instantiate(prefabCraftingSlot) : recipeParent.GetChild(i).gameObject;
            CraftingSlot craftingSlot = recipeChild.GetComponent<CraftingSlot>();
            craftingSlot.Initialize(visibleRecipes[i], SelectCraftingRecipe);
        }
    }

    public void Initialize()
    {
        UpdateCraftingMenu();
    }

    public void CraftItem()
    {
        
    }
}

public class CraftingData
{
    bool isCraftable;
    CraftingRecipe craftingRecipe;
    ItemStack[] availableIngredients;

    public CraftingData(bool isCraftable, CraftingRecipe craftingRecipe, ItemStack[] availableIngredients)
    {
        this.isCraftable = isCraftable;
        this.craftingRecipe = craftingRecipe;
        this.availableIngredients = availableIngredients;
    }

    public bool IsCraftable { get => isCraftable; }
    public CraftingRecipe CraftingRecipe { get => craftingRecipe; }
    public ItemStack[] AvailableIngredients { get => availableIngredients; }
}