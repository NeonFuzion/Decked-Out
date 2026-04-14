using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] Item output;
    [SerializeField] ItemStack[] ingredients;

    public Item Output { get => output; }
    public ItemStack[] Ingredients { get => ingredients; }
}
