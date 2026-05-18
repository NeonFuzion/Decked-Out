using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float lerpSpeed = 10f;

    Transform followTarget;
    float targetFill;

    void Start()
    {
        targetFill = 1;
        fill.fillAmount = targetFill;
    }

    void Update()
    {
        if (followTarget) transform.parent.position = followTarget.position;

        if (Mathf.Abs(fill.fillAmount - targetFill) < 0.01f) return;
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
    }

    public void Initialize(float fillAmount, Transform followTarget = null)
    {
        this.followTarget = followTarget;
        targetFill = fillAmount;
        fill.fillAmount = fillAmount;
    }

    public void SetFill(float percentage)
    {
        targetFill = Mathf.Clamp01(percentage);
    }
}
