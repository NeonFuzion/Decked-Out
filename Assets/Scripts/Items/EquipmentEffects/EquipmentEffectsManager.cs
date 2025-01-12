using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentEffectsManager : MonoBehaviour
{
    [SerializeField] UnityEvent onDamageDealt, onDamageTaken, onDash, onKill;

    List<TimerPair> timePairs;
    List<EquipmentEffect> equipmentEffects;

    public UnityEvent OnDamageDealt { get => onDamageDealt; }
    public UnityEvent OnDamageTaken { get => onDamageTaken; }
    public UnityEvent OnDash { get => onDash; }
    public UnityEvent OnKill { get => onKill; }

    private void Awake()
    {
        timePairs = new List<TimerPair>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < timePairs.Count; i++)
        {
            TimerPair pair = timePairs[i];
            pair.IncrementTime(Time.deltaTime);

            if (pair.Time > 0) return;
            timePairs.Remove(pair);
            i--;
            pair.OnFinish?.Invoke();
        }
    }

    public void AddTimerPair(UnityAction unityEvent, float waitTime, EquipmentEffect equipmentEffect)
    {
        timePairs.Add(new TimerPair(waitTime, unityEvent, equipmentEffect));
    }

    public void InvokeOnDamageDealt() => onDamageDealt?.Invoke();

    public void InvokeOnDamageTaken() => onDamageTaken?.Invoke();

    public void InvokeOnDash() => onDash?.Invoke();

    public void InvokeOnKill() => onKill?.Invoke();
}

public class TimerPair
{
    float time;
    UnityEvent onFinish;
    EquipmentEffect equipmentEffect;

    public TimerPair(float time, UnityAction unityAction, EquipmentEffect equipmentEffect)
    {
        this.time = time;
        this.equipmentEffect = equipmentEffect;

        onFinish = new UnityEvent();
        onFinish.AddListener(unityAction);
    }

    public float Time { get => time; }
    public UnityEvent OnFinish { get => onFinish; }
    public EquipmentEffect EquipmentEffect { get => equipmentEffect; }

    public void IncrementTime(float increment)
    {
        time -= increment;
    }

    public void AddTime(float increment)
    {
        time += increment;
    }
}