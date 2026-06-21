using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] bool startActive;
    [SerializeField] UnityEvent onTimerCompleted;

    float timerEndTime;
    bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!startActive) return;
        SetTimer(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (Time.time < timerEndTime) return;
        isActive = false;
        onTimerCompleted?.Invoke();
    }

    public void SetTimer(float duration)
    {
        isActive = true;
        timerEndTime = Time.time + duration;
    }
}
