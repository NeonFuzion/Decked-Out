using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] bool startActive;
    [SerializeField] float duration;
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
        this.duration = duration;
        RestartTimer();
    }

    public void RestartTimer()
    {
        timerEndTime = Time.time + duration;
        isActive = true;
    }

    public void AddTimerEndListener(UnityAction unityAction)
    {
        onTimerCompleted?.AddListener(unityAction);
    }
}
