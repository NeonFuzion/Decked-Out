using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentEffectsManager : MonoBehaviour
{
    [SerializeField] UnityEvent onDamageDealt, onDamageTaken, onDash, onKill;

    List<TimerPair> timePairs;
    List<PassiveEffect> passiveEffects;
    List<SetBonus> setBonuses;

    public UnityEvent OnDamageDealt { get => onDamageDealt; }
    public UnityEvent OnDamageTaken { get => onDamageTaken; }
    public UnityEvent OnDash { get => onDash; }
    public UnityEvent OnKill { get => onKill; }

    private void Awake()
    {
        timePairs = new List<TimerPair>();
        passiveEffects = new List<PassiveEffect>();
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

    public void AddTimerPair(UnityAction unityEvent, float waitTime, PassiveEffect equipmentEffect)
    {
        timePairs.Add(new (waitTime, unityEvent, equipmentEffect));
    }

    public void RemoveAllEffects()
    {
        onDamageDealt.RemoveAllListeners();
        onDamageTaken.RemoveAllListeners();
        onDash.RemoveAllListeners();
        onKill.RemoveAllListeners();

        timePairs.Clear();
        passiveEffects.Clear();
    }

    public void AddPassiveEffect(PassiveEffect equipmentEffect)
    {
        passiveEffects.Add(equipmentEffect);
    }

    public void RemovePassiveEffect(PassiveEffect equipmentEffect)
    {
        passiveEffects.Remove(equipmentEffect);
    }

    public void AddSetBonusEffect(SetBonus setBonus)
    {
        setBonuses.Add(setBonus);
    }

    public void RemoveSetBonusEffect(SetBonus setBonus)
    {
        setBonuses.Remove(setBonus);
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
    PassiveEffect passiveEffect;

    public TimerPair(float time, UnityAction unityAction, PassiveEffect passiveEffect)
    {
        this.time = time;
        this.passiveEffect = passiveEffect;

        onFinish = new UnityEvent();
        onFinish.AddListener(unityAction);
    }

    public float Time { get => time; }
    public UnityEvent OnFinish { get => onFinish; }
    public PassiveEffect PassiveEffect { get => passiveEffect; }

    public void IncrementTime(float increment)
    {
        time -= increment;
    }

    public void AddTime(float increment)
    {
        time += increment;
    }
}