using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/CraftingRecipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [SerializeField] ItemSO output;
    [SerializeField] ItemStack[] ingredients;

    public ItemSO Output { get => output; }
    public ItemStack[] Ingredients { get => ingredients; }
}
