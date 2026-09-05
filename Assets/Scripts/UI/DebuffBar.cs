using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuffBar : MonoBehaviour
{
    [SerializeField] Transform debuffBarParent;
    [SerializeField] GameObject debuffIconPrefab;

    readonly Dictionary<Debuff, GameObject> icons = new ();

    public void IncrementDebuff(Debuff debuff, int stackCount)
    {
        if (stackCount > 0)
        {
            GameObject icon;
            if (icons.ContainsKey(debuff))
            {
                icon = icons[debuff];
            }
            else
            {
                icon = DebuffIconObjectPool.Instance.RetrieveItem();
                icons.Add(debuff, icon);
                icon.SetActive(true);

                icon.GetComponentInChildren<Image>().sprite = debuff.Sprite;
                icon.transform.SetParent(debuffBarParent);
                icon.transform.localScale = Vector3.one;
            }
            icon.GetComponentInChildren<TextMeshProUGUI>().text = stackCount.ToString();
        }
        else
        {
            GameObject icon = icons[debuff];
            icon.transform.SetParent(null);
            DebuffIconObjectPool.Instance.ReturnItem(icon);
        }
    }
}
