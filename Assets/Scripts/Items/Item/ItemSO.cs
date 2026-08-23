using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Material")]
public class ItemSO : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] [TextArea(1, 10)] string description;
    [SerializeField] Sprite sprite;
    [SerializeField] ItemStack[] ingredients;

    public string ItemName { get => itemName; }
    public string Description { get => description; }
    public Sprite Sprite { get => sprite; }
    public ItemStack[] Ingredients { get => ingredients; }
}
