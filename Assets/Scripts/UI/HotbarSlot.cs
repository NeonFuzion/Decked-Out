using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] Sprite emptySprite;
    [SerializeField] TextMeshProUGUI inputText;
    [SerializeField] GameObject cooldownHolder;
    [SerializeField] Image mainHandImage;

    bool isEmpty;

    public bool IsEmpty { get => isEmpty; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSprite(MainHand mainHand)
    {
        mainHandImage.sprite = emptySprite;
        isEmpty = !mainHand;

        if (isEmpty) return;
        mainHandImage.sprite = mainHand.Sprite;
        mainHandImage.SetNativeSize();
    }
}
