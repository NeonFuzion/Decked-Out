using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigger1, onTrigger2, onTrigger3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerEvent1()
    {
        onTrigger1?.Invoke();
    }

    public void TriggerEvent2()
    {
        onTrigger2?.Invoke();
    }
    public void TriggerEvent3()
    {
        onTrigger3?.Invoke();
    }
}
