using UnityEngine;

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] Item output;
    [SerializeField] ItemStack[] ingredients;

    public Item Output { get => output; }
    public ItemStack[] Ingredients { get => ingredients; }
}
