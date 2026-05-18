using UnityEngine;
using UnityEngine.UI;

public class StaggerBar : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color refillingColor = Color.yellow;

    float targetFill;

    

    void Start()
    {
        targetFill = 1f;
        fill.fillAmount = targetFill;
    }

    void Update()
    {
        if (Mathf.Abs(fill.fillAmount - targetFill) < 0.01f) return;
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
    }

    public void Initialize(float fillAmount)
    {
        targetFill = fillAmount;
        fill.fillAmount = fillAmount;
    }

    public void SetFill(float percentage)
    {
        targetFill = Mathf.Clamp01(percentage);
    }

    public void SetRefilling(bool refilling)
    {
        fill.color = refilling ? refillingColor : normalColor;
    }
}
