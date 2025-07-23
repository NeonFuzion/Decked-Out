using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TreasureChest : TerrainObject
{
    [SerializeField] UnityEvent onOpen;

    bool opening;

    TreasureChestData treasureChestData;

    public UnityEvent OnOpen { get => onOpen; }

    // Start is called before the first frame update
    void Start()
    {
        opening = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if (opening) return;
        opening = true;
        GetComponent<Animator>().CrossFade("OpenChest", 0, 0);
    }

    void OpenChest()
    {
        onOpen?.Invoke();
        treasureChestData.CurrentTreasureChestState = TreasureChestData.TreasureChestState.Opened;
        gameObject.SetActive(false);
    }

    void SpawnChest()
    {
        treasureChestData.CurrentTreasureChestState = TreasureChestData.TreasureChestState.Spawned;
        gameObject.SetActive(true);
    }

    public override RoomObjectData Initialize(DungeonGenerator dungeonGenerator)
    {
        return new TreasureChestData(false, new(), new(), dungeonGenerator.LootPool.ToList());
    }

    public override void LoadData(RoomObjectData roomObjectData, DungeonGenerator dungeonGenerator)
    {
        treasureChestData = roomObjectData as TreasureChestData;

        LootDrops script = GetComponent<LootDrops>();
        script.CommonDrops = treasureChestData.CommonDrops;
        script.RareDrops = treasureChestData.RareDrops;
        script.SingleDrops = treasureChestData.SingleDrops;

        if (treasureChestData.CurrentTreasureChestState == TreasureChestData.TreasureChestState.None)
            treasureChestData.CurrentTreasureChestState = TreasureChestData.TreasureChestState.Unspawned;

        switch (treasureChestData.CurrentTreasureChestState)
        {
            case TreasureChestData.TreasureChestState.Unspawned:
                EventManager.AddOnCombatEndedListener(SpawnChest);
                gameObject.SetActive(false);
                break;
            case TreasureChestData.TreasureChestState.Opened:
                gameObject.SetActive(false);
                break;
        }
    }
}

public class TreasureChestData : RoomObjectData
{
    public TreasureChestState CurrentTreasureChestState;
    public List<Item> CommonDrops, RareDrops, SingleDrops;

    public TreasureChestData(bool isActive, List<Item> commonDrops, List<Item> rareDrops, List<Item> singleDrops) : base(isActive)
    {
        CurrentTreasureChestState = TreasureChestState.Unspawned;

        CommonDrops = commonDrops;
        RareDrops = rareDrops;
        SingleDrops = singleDrops;
    }

    public enum TreasureChestState { None, Unspawned, Spawned, Opened }
}