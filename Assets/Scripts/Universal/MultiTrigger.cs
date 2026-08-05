using UnityEngine;
using UnityEngine.Events;

public class MultiTrigger : MonoBehaviour
{
    [SerializeField] int triggerCount;
    [SerializeField] UnityEvent onTrigger, onFinish;

    int currentTriggerCount;

    public UnityEvent OnTrigger => onTrigger;
    public UnityEvent OnFinish => onFinish; 

    public void Trigger()
    {
        if (currentTriggerCount >= triggerCount) return;
        onTrigger?.Invoke();
        currentTriggerCount++;

        if (currentTriggerCount != triggerCount) return;
        onFinish?.Invoke();
    }

    public void SetTriggerCount(int triggerCount)
    {
        this.triggerCount = triggerCount;
    }
}
