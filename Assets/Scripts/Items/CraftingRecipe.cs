using UnityEngine;

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] Item output;
    [SerializeField] Item[] ingredients;

    public Item Output { get => output; }
    public Item[] Ingredients { get => ingredients; }
}
