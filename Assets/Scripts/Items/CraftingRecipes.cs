using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/CraftingRecipes")]
public class CraftingRecipes : ScriptableObject
{
    [SerializeField] CraftingRecipe[] recipes;

    public CraftingRecipe[] Recipes { get => recipes; }
}