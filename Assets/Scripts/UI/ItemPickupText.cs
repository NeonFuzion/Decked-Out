using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemPickupText : MonoBehaviour
{
    float pickupRemoveTime, curPickupLifetime;
    TextMeshProUGUI pickupText;

    // Start is called before the first frame update
    void Start()
    {
        pickupRemoveTime = 3;
        curPickupLifetime = 0;
        pickupText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

        curPickupLifetime += Time.deltaTime;
        if (curPickupLifetime < pickupRemoveTime) return;
        PickupTimeout();
    }
    /*
    public void UpdatePickupText(ItemBase item)
    {
        Dictionary<string, int> pickups = new Dictionary<string, int>();

        foreach (string line in pickupText.text.Split("\n"))
        {
            string[] split = line.Split(": ");
            if (line.Length == 0) break;
            pickups.Add(split[0], int.Parse(split[1]));
        }
        if (pickups.ContainsKey(item.ItemName)) pickups[item.ItemName]++;
        else pickups.Add(item.ItemName, 1);
        if (pickups.Count > 5) pickups.Remove(pickups.Keys.ToArray()[0]);

        pickupText.SetText("");
        foreach (KeyValuePair<string, int> kpv in pickups)
        {
            pickupText.text += kpv.Key + ": " + kpv.Value;
            if (kpv.Key == pickups.Keys.ToArray()[pickups.Count - 1]) break;
            pickupText.text += "\n";
        }
    }*/

    void PickupTimeout()
    {
        string[] pickups = pickupText.text.Split("\n");
        pickupText.SetText("");
        for (int i = 1; i < pickups.Length; i++) pickupText.text += pickups[i] + "\n";
        curPickupLifetime = 0;
    }
}
