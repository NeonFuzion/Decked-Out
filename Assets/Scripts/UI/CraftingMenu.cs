using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    [SerializeField] ItemLoader allRecipes;
    [SerializeField] Transform materialsParent, recipeParent;
    [SerializeField] CraftingSlot selectedCraftableSlot;
    [SerializeField] GameObject prefabQuotaSlot, prefabCraftingSlot;

    Inventory inventory;
    CraftingData currentCraftingData;
    List<CraftingData> visibleRecipes;

    void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectCraftingRecipe(CraftingData craftingData)
    {
        currentCraftingData = craftingData;
        ItemSO currentRecipe = craftingData.ItemSO;
        selectedCraftableSlot.UpdateItem(currentRecipe.Sprite, 0);

        for (int i = 0; i < currentRecipe.Ingredients.Length; i++)
        {
            GameObject materialChild = i >= materialsParent.childCount ? Instantiate(prefabQuotaSlot, materialsParent) : materialsParent.GetChild(i).gameObject;
            materialChild.SetActive(true);
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
        // Hide old recipes & ingredients
        foreach (Transform material in materialsParent) material.gameObject.SetActive(false);
        foreach (Transform recipe in recipeParent) recipe.gameObject.SetActive(false);
        selectedCraftableSlot.ResetItem();

        // Find all recipes that contain materials that exist in inventory even if they can't be fully crafted
        visibleRecipes.Clear();
        allRecipes.Items.ForEach(recipe =>
        {
            bool isCraftable = true;
            bool isMaterialFound = false;
            List<ItemStack> availableIngredients = new ();
            recipe.Ingredients.ToList().ForEach(ingredient =>
            {
                ItemStack inventoryStack = inventory.FindItem(ingredient.Item);

                int amount = inventoryStack != null ? inventoryStack.Amount : 0;
                if (amount < ingredient.Amount) isCraftable = false;
                if (amount > 0) isMaterialFound = true;

                availableIngredients.Add(new (ingredient.Item, amount));
            });

            if (!isMaterialFound) return;
            visibleRecipes.Add(new (isCraftable, recipe, availableIngredients.ToArray()));
        });

        // Populate recipes
        for (int i = 0; i < visibleRecipes.Count; i++)
        {
            GameObject recipeChild = i >= recipeParent.childCount ? Instantiate(prefabCraftingSlot, recipeParent) : recipeParent.GetChild(i).gameObject;
            if (!recipeChild.activeInHierarchy) recipeChild.SetActive(true);
            CraftingSlot craftingSlot = recipeChild.GetComponent<CraftingSlot>();
            craftingSlot.Initialize(visibleRecipes[i], SelectCraftingRecipe);
        }
    }

    public void Initialize()
    {
        if (!inventory)
        {
            inventory = Inventory.Instance;
        }
        if (visibleRecipes == null)
        {
            visibleRecipes = new ();
        }

        UpdateCraftingMenu();
    }

    public void CraftItem()
    {
        if (currentCraftingData == null) return;
        if (!currentCraftingData.IsCraftable) return;
        ItemSO currentItem = currentCraftingData.ItemSO;
        currentItem.Ingredients.ToList().ForEach(stack =>
        {
            inventory.RemoveItem(stack.Item, stack.Amount);
        });
        inventory.AddItem(currentItem);
        UpdateCraftingMenu();
        CraftingData data = visibleRecipes.Find(data => data.ItemSO == currentItem);

        if (data == null) return;
        SelectCraftingRecipe(data);
    }
}

public class CraftingData
{
    bool isCraftable;
    ItemSO craftingRecipe;
    ItemStack[] availableIngredients;

    public CraftingData(bool isCraftable, ItemSO itemSO, ItemStack[] availableIngredients)
    {
        this.isCraftable = isCraftable;
        this.craftingRecipe = itemSO;
        this.availableIngredients = availableIngredients;
    }

    public bool IsCraftable { get => isCraftable; }
    public ItemSO ItemSO { get => craftingRecipe; }
    public ItemStack[] AvailableIngredients { get => availableIngredients; }
}