using UnityEngine;

public class Being : MonoBehaviour
{
    public BeingType BeingType { get; protected set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum BeingType { None, Friendly, Hostile }