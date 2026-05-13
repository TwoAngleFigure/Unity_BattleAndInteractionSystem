using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    #region Field
    [Header("Datas")]
    private Dictionary<string, Currency> _currencyInventory = new();
    private Dictionary<string, ItemData> _itemInventory = new();

    [Header("Action")]
    public Action<Currency[]> CurrencyChangedAction;
    public Action<ItemData[]> InventoryChangedAction;
    public Action<bool> OnInventoryToggled;

    [Header("UI")]
    [SerializeField] private CanvasGroup _inventoryUI;

    [Header("Getter")]
    public static PlayerInventory Instance { get; private set; }
    public Currency[] CurrencyInventory => _currencyInventory.Values.ToArray();
    public ItemData[] ItemInventory => _itemInventory.Values.ToArray();
    public bool InventoryToggleState { get; private set; }
    #endregion

    #region Unity Lifecycle
    public void Initailize()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        SetInventoryUIState(InventoryToggleState);
        CurrencyChangedAction?.Invoke(_currencyInventory.Values.ToArray());
    }
    #endregion

    #region Currency Database
    public void AddCurrency(string name, int count)
    {
        if (_currencyInventory.TryGetValue(name, out Currency targetCurrency))
        {
            targetCurrency.AddCount(count);
            CurrencyChangedAction?.Invoke(new[] { targetCurrency });
        }
    }

    public bool RemoveCurrency(string name, int count)
    {
        if (_currencyInventory.TryGetValue(name, out Currency targetCurrency) == false) return false;
        if (targetCurrency.RemoveCount(count) == false) return false;
        CurrencyChangedAction?.Invoke(new[] { targetCurrency });
        return true;
    }

    public void SetCurrencyData(Currency currency)
    {
        _currencyInventory.Add(currency.currencyType.ToString(), currency);
    }
    #endregion

    #region Inventory Item Database
    public void AddItem(ItemData newItem)
    {
        if (_itemInventory.TryGetValue(newItem.id, out ItemData oldItem))
             oldItem.count += newItem.count;
        else
            _itemInventory.Add(newItem.id, newItem);
        InventoryChangedAction?.Invoke(_itemInventory.Values.ToArray());
    }

    public bool UseItem(string id)
    {
        if (_itemInventory.TryGetValue(id, out ItemData oldItem))
        {
            oldItem.count -= 1;
            if (oldItem.count <= 0) _itemInventory.Remove(id);
            oldItem.Use(BasePlayer.Instance.gameObject);
            InventoryChangedAction?.Invoke(_itemInventory.Values.ToArray());
            return true;
        }
        return false;
    }

    public ItemData GetItemData(string id)
    {
        if (_itemInventory.TryGetValue(id, out ItemData itemData))
            return itemData;
        return null;
    }
    #endregion

    #region Inventory UI Control

    private void OnInventory(InputValue value)
    {
        if (value.isPressed == false) return;
        SetInventoryUIState(!InventoryToggleState);
        InventoryChangedAction?.Invoke(_itemInventory.Values.ToArray());
        CurrencyChangedAction?.Invoke(_currencyInventory.Values.ToArray());
    }

    private void SetInventoryUIState(bool state)
    {
        if (state)
        {
            InventoryToggleState = true;
            _inventoryUI.alpha = 1;
            _inventoryUI.interactable = true;
        }
        else
        {
            InventoryToggleState = false;
            _inventoryUI.alpha = 0;
            _inventoryUI.interactable = false;
        }
        OnInventoryToggled?.Invoke(InventoryToggleState);
    }
    #endregion
}