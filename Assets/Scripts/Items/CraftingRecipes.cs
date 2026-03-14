using System;
using UnityEngine;

[CreateAssetMenu]
public class CraftingRecipes : ScriptableObject
{
    [SerializeField] CraftingRecipe[] recipes;

    public CraftingRecipe[] Recipes { get => recipes; }
}