using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Item Slot")]
    public GameObject itemSlotShopPrefab;
    private GridLayoutGroup slotParent;
    private List<ItemSlotShopUI> itemSlotUIs = new();

    [Header("Item Select Page")]
    private ItemSelectPageShopUI itemSelectPageShop;

    private CanvasGroup canvasGroup;

    public void Initialize(ShopSystem owner)
    {
        if (slotParent == null) slotParent = GetComponentInChildren<GridLayoutGroup>();
        if (itemSelectPageShop == null) itemSelectPageShop = GetComponentInChildren<ItemSelectPageShopUI>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        owner.InventoryChangedAction += UpdateItemUI;
        itemSelectPageShop.SetButtonEvent(() => owner.BuyItem(itemSelectPageShop.CurrentItemId));

        SetUIActive(false);
    }

    #region Item UI
    public void UpdateItemUI(GoodsData[] goodsDatas)
    {
        //itemDatas 개수 만큼 SlotUI 활성화
        for (int i = 0; i < goodsDatas.Length; i++)
        {
            if (i >= itemSlotUIs.Count)
            {
                itemSlotUIs.Add(CreateNewItemSlot());
            }

            GoodsData currentItem = goodsDatas[i];
            itemSlotUIs[i].ActiveSlot(currentItem.itemData.icon, currentItem.itemData.count.ToString(), () => 
            {
                itemSelectPageShop.ActiveSelectPage(
                    currentItem.itemData.id, 
                    currentItem.itemData.icon, 
                    currentItem.itemData.itemName, 
                    currentItem.itemData.count.ToString(), 
                    currentItem.itemData.value.ToString(),
                    currentItem.itemData.description, 
                    true
                );
                itemSelectPageShop.SetCurrencyIcon(currentItem.targetCurrency);
            });
            itemSlotUIs[i].SetCurrencyIcon(goodsDatas[i].targetCurrency);
        }

        for (int i = goodsDatas.Length; i < itemSlotUIs.Count; i++)
        {
            itemSlotUIs[i].DesableSlot();
        }

        RefreshItemSelectPage(goodsDatas);
    }

    private void RefreshItemSelectPage(GoodsData[] goodsDatas)
    {
        string selectedId = itemSelectPageShop.CurrentItemId;
        if (!string.IsNullOrEmpty(selectedId))
        {
            ItemData currentSelected = null;
            foreach(var item in goodsDatas)
            {
                if (item.itemData.id == selectedId) { currentSelected = item.itemData; break; }
            }

            if (currentSelected != null && currentSelected.count > 0)
            {
                itemSelectPageShop.ActiveSelectPage(
                    currentSelected.id,
                    currentSelected.icon,
                    currentSelected.itemName,
                    currentSelected.count.ToString(),
                    currentSelected.value.ToString(),
                    currentSelected.description,
                    true
                );
            }
            else
            {
                itemSelectPageShop.DisableSelectPage();
            }
        }
        else
        {
            itemSelectPageShop.DisableSelectPage();
        }
    }

    public ItemSlotShopUI CreateNewItemSlot()
    {
        ItemSlotShopUI newSlot = Instantiate(itemSlotShopPrefab).GetComponent<ItemSlotShopUI>();
        newSlot.transform.SetParent(slotParent.gameObject.transform, false);
        return newSlot;
    }
    #endregion

    public void SetUIActive(bool active)
    {
        if(active)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
        }
    }
}
