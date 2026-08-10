using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float lerpSpeed = 10f;

    float targetFill;
    Transform followTarget;

    void Start()
    {
        targetFill = 1;
        fill.fillAmount = targetFill;
    }

    void Update()
    {
        if (followTarget) transform.position = followTarget.position;

        if (Mathf.Abs(fill.fillAmount - targetFill) < 0.01f) return;
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
    }

    public void Initialize(Transform followTarget = null, int currentHealth = 1)
    {
        this.followTarget = followTarget;

        targetFill = currentHealth;
        fill.fillAmount = currentHealth;
    }

    public void SetFill(float percentage)
    {
        targetFill = Mathf.Clamp01(percentage);
    }
}
