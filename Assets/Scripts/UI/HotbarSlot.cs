using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] Sprite emptySprite;
    [SerializeField] TextMeshProUGUI inputText, cooldownText;
    [SerializeField] Image image, cooldownImage;

    bool isEmpty;
    float cooldown, currentCooldown;

    public bool IsEmpty { get => isEmpty; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown <= 0) return;
        currentCooldown -= Mathf.Min(Time.deltaTime, currentCooldown);
        cooldownImage.fillAmount = currentCooldown / cooldown;
        cooldownText.SetText(GetRoundedCooldown(cooldown));

        if (currentCooldown > 0) return;
        cooldownImage.fillAmount = 0;
        cooldownText.SetText("");
    }

    string GetRoundedCooldown(float currentCooldown)
    {
        return System.Math.Round(currentCooldown, 1) + "";
    }

    public void Initialize(Sprite sprite, float cooldown)
    {
        image.sprite = emptySprite;
        isEmpty = !sprite;

        if (isEmpty) return;
        this.cooldown = cooldown;
        image.sprite = sprite;
        image.SetNativeSize();
    }

    public void StartCooldown()
    {
        currentCooldown = cooldown;
        cooldownImage.fillAmount = 1;
        cooldownText.SetText(GetRoundedCooldown(cooldown));
    }
}
