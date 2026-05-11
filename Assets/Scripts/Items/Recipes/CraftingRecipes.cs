using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/CraftingRecipes")]
public class CraftingRecipes : ScriptableObject
{
    [SerializeField] CraftingRecipeSO[] recipes;

    public CraftingRecipeSO[] Recipes { get => recipes; }
}