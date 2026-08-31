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
            if (!icons.TryGetValue(debuff, out GameObject icon))
            {
                icon = DebuffIconObjectPool.Instance.RetrieveItem();
                icon.SetActive(true);
                icon.transform.SetParent(debuffBarParent);
                icon.GetComponent<Image>().sprite = debuff.Sprite;
                icons.Add(debuff, icon);
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
