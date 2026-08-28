using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuffBar : MonoBehaviour
{
    [SerializeField] Transform debuffBarParent;
    [SerializeField] GameObject debuffIconPrefab;

    readonly Dictionary<Sprite, GameObject> icons = new ();

    public void AddDebuff(Sprite sprite, int stackCount)
    {
        if (!icons.TryGetValue(sprite, out GameObject icon))
        {
            icon = Instantiate(debuffIconPrefab, debuffBarParent);
            icon.GetComponent<Image>().sprite = sprite;
            icons[sprite] = icon;
        }
        icon.GetComponentInChildren<TextMeshProUGUI>().text = stackCount.ToString();
    }

    public void RemoveDebuff(Sprite sprite, int stackCount)
    {
        if (!icons.TryGetValue(sprite, out GameObject icon)) return;
        if (stackCount <= 0)
        {
            Destroy(icon);
            icons.Remove(sprite);
        }
        else
        {
            icon.GetComponentInChildren<TextMeshProUGUI>().text = stackCount.ToString();
        }
    }
}
