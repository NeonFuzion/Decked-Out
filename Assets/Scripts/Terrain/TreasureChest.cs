using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] UnityEvent onOpen;

    bool opening;

    // Start is called before the first frame update
    void Start()
    {
        opening = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if (opening) return;
        opening = true;
        GetComponent<Animator>().CrossFade("OpenChest", 0, 0);
    }

    void OpenChest()
    {
        onOpen?.Invoke();
        Destroy(gameObject);
    }
}
