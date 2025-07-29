using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : RoomObject
{
    [SerializeField] Item scrip;
    [SerializeField] Item[] items;

    Transform player;
    TextMeshProUGUI itemText, rentText;
    Image image;
    QuestGiverData questGiverData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    QuestGiverData GenerateQuest(int amount)
    {
        return new(true, amount + Random.Range(5, 9), items[Random.Range(0, items.Length)]);
    }

    void OnClick()
    {
        itemText.SetText($"{questGiverData.Item.ItemName} x{questGiverData.Amount}");
        image.sprite = questGiverData.Item.Sprite;
    }

    void SubmitItems()
    {
        Inventory script = player.GetComponent<Inventory>();

        if (!script.Items.RemoveItem(questGiverData.Item, questGiverData.Amount)) return;
        questGiverData = GenerateQuest(questGiverData.Amount);
        script.AddItem(scrip, 3);
        OnClick();
    }

    public void PayRent()
    {
        Inventory script = player.GetComponent<Inventory>();

        if (!script.Items.RemoveItem(scrip, 200)) return;
        rentText.SetText("Good Work! You will be rich in no time at this pace :)");
    }

    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        return GenerateQuest(10);
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        questGiverData = roomObjectData as QuestGiverData;

        FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().ForEach(gm =>
        {
            if (gm.GetComponent<Player>())
            {
                player = gm.transform;
            }
            else if (gm.GetComponent<Canvas>())
            {
                int index = gm.transform.childCount - 1;
                Transform sadUI = gm.transform.GetChild(index);
                GetComponent<Clickable>().OnClick.AddListener(() =>
                {
                    gm.GetComponent<UIManager>().OpenMenu(sadUI.gameObject);
                    player.GetComponent<PlayerInput>().MenuSetup();
                });

                Transform questUI = sadUI.GetChild(0);
                itemText = questUI.GetChild(1).GetComponent<TextMeshProUGUI>();
                image = questUI.GetChild(3).GetComponent<Image>();
                questUI.GetChild(2).GetComponent<Button>().onClick.AddListener(SubmitItems);

                Transform rentUI = sadUI.GetChild(1);
                rentUI.GetChild(2).GetComponent<Button>().onClick.AddListener(PayRent);
                rentText = rentUI.GetChild(1).GetComponent<TextMeshProUGUI>();
            }
        });
    }
}

public class QuestGiverData : RoomObjectData
{
    public int Amount;
    public Item Item;

    public QuestGiverData(bool isActive, int amount, Item item) : base(isActive)
    {
        Amount = amount;
        Item = item;
    }
}