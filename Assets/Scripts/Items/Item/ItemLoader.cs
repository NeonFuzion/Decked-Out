using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/All")]
public class ItemLoader : ScriptableObject
{
    [SerializeField] List<ItemSO> items;

    public List<ItemSO> Items => items;
}
