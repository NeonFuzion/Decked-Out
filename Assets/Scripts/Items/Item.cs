using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Material")]
public class Item : ScriptableObject
{
    [SerializeField] string itemName, description;
    [SerializeField] Sprite sprite;
    [SerializeField] Item[] craftingRecipe;

    public string ItemName { get => itemName; }
    public string Description { get => description; }
    public Sprite Sprite { get => sprite; }
}
