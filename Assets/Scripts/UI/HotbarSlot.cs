using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputText, cooldownText, amountText;
    [SerializeField] Image image, cooldownImage;
    [SerializeField] GameObject emptyImage;

    bool isEmpty;
    float cooldown, currentCooldown;
    int amount;

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
        cooldownText.SetText(GetRoundedCooldown(currentCooldown));

        if (currentCooldown > 0) return;
        cooldownImage.fillAmount = 0;
        cooldownText.SetText("");
    }

    string GetRoundedCooldown(float currentCooldown)
    {
        return System.Math.Round(currentCooldown, 1) + "";
    }

    public void Initialize(Sprite sprite, float cooldown, int amount)
    {
        isEmpty = !sprite;

        if (isEmpty)
        {
            emptyImage.SetActive(true);
            image.gameObject.SetActive(false);
            amountText?.gameObject.SetActive(false);
        }
        else
        {
            this.cooldown = cooldown;
            this.amount = amount;
            image.sprite = sprite;

            image.SetNativeSize();
            image.gameObject.SetActive(true);
            emptyImage.SetActive(false);

            if (!amountText) return;
            amountText.SetText(amount + "");
            amountText.gameObject.SetActive(true);
        }
    }

    public void StartCooldown()
    {
        currentCooldown = cooldown;
        cooldownImage.fillAmount = 1;
        cooldownText.SetText(GetRoundedCooldown(cooldown));
    }

    public void IncrementAmount()
    {
        if (--amount > 0) amountText.SetText(amount + "");
        else Initialize(null, 0, 0);
    }
}
