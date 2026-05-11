using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSystem : MonoBehaviour, IInteractive
{
    #region Field
    [Header("Datas")]
    public List<GoodsDataOrigin> GoodsList = new();
    private Dictionary<string, GoodsData> _goodsInventory = new();

    [Header("Action")]
    //Database Changed
    public Action<GoodsData[]> InventoryChangedAction;

    [Header("UI")]
    [SerializeField] private ShopUI _shopUI;
    public GameObject UIObject;

    [Header("Getter")]
    public ItemData[] ItemInventory => GetItemDataArray();
    #endregion

    public void Awake()
    {
        if(_shopUI == null) UIObject.GetComponentInChildren<ShopUI>();
        _shopUI.Initialize(this);
        RefillGoods();
    }

    public void RefillGoods()
    {
        foreach (GoodsDataOrigin data in GoodsList)
        {
            ItemDataOrigin origin = data.itemDataOrigin;
            ItemData goods = new ItemData
            (
                origin.id,
                origin.name,
                origin.description,
                origin.value,
                data.count,
                origin.itemType,
                origin.icon,
                origin.effects
            );

            _goodsInventory.Add(goods.id, new GoodsData
            {
                itemData = goods,
                targetCurrency = data.targetCurrency
            });
        }
        InventoryChangedAction?.Invoke(_goodsInventory.Values.ToArray());
    }

    public ItemData RemoveItem(string id)
    {
        if (_goodsInventory.TryGetValue(id, out GoodsData targetGoods))
        {
            ItemData targetItem = targetGoods.itemData;
            targetItem.count -= 1;
            if (targetItem.count <= 0) _goodsInventory.Remove(id);

            InventoryChangedAction?.Invoke(_goodsInventory.Values.ToArray());
            return targetItem;
        }
        return null;
    }

    public void BuyItem(string targetItemId)
    {
        if (!_goodsInventory.TryGetValue(targetItemId, out GoodsData goodsData))
        {
            Debug.LogWarning($"Item {targetItemId} not found in shop!");
            return;
        }

        CurrencyType targetCurrency = goodsData.targetCurrency;
        ItemData item = goodsData.itemData;

        if (PlayerInventory.Instance.RemoveCurrency(targetCurrency.ToString(), item.value))
        {
            ItemData clonedItem = item.Clone(overrideCount: 1);

            PlayerInventory.Instance.AddItem(clonedItem);
            RemoveItem(item.id);
        }
        else
        {
            Debug.Log($"Not Enough {goodsData.targetCurrency}");
        }
    }

    public ItemData[] GetItemDataArray()
    {
        ItemData[] datas = new ItemData[_goodsInventory.Count];
        int i = 0;
        foreach (GoodsData data in _goodsInventory.Values)
        {
            datas[i] = data.itemData; i++;
        }
        return datas;
    }

    public string InteractionType() => "Shop";

    public void Interaction()
    {
        _shopUI.SetUIActive(true);
    }

    public void Clear()
    {
        _shopUI.SetUIActive(false);
    }
}

//인스팩터 편집용 데이터 셋
[System.Serializable]
public struct GoodsDataOrigin
{
    public ItemDataOrigin itemDataOrigin;
    public CurrencyType targetCurrency;
    public int count; //상점에서 보유중인 물건의 개수
}

public struct GoodsData
{
    public ItemData itemData;
    public CurrencyType targetCurrency;
}