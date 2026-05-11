using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Currency")]
    public GameObject currencySlotPrefab;
    private Dictionary<string, CurrencySlotUI> currencySlotUI = new();

    [Header("Item Slot")]
    public GameObject itemSlotPrefab;
    private GridLayoutGroup slotParent;
    private List<ItemSlotUI> itemSlotUIs = new();

    [Header("Sorting")]
    public SortingType currentSortingType = SortingType.TimeAscending;
    private TMP_Dropdown sortDropdown;

    [Header("Item Select Page")]
    private ItemSelectPageUI itemSelectPage;

    public void Start()
    {
        PlayerInventory.Instance.CurrencyChangedAction += UpdateCurrencyUI;
        PlayerInventory.Instance.InventoryChangedAction += UpdateItemUI;

        foreach (var slot in GetComponentsInChildren<CurrencySlotUI>())
        {
            if (string.IsNullOrEmpty(slot.currencyId)) continue;

            if (currencySlotUI.ContainsKey(slot.currencyId))
            {
                Debug.LogWarning($"Duplicate currency slot detected for ID: {slot.currencyId}. Only the first one will be used.");
                continue;
            }
            currencySlotUI.Add(slot.currencyId, slot);
        }

        if (slotParent == null) slotParent = GetComponentInChildren<GridLayoutGroup>();

        if (sortDropdown == null) sortDropdown = GetComponentInChildren<TMP_Dropdown>();
        sortDropdown.value = (int)currentSortingType;
        sortDropdown.onValueChanged.AddListener(OnSortDropdownValueChanged);

        if (itemSelectPage == null) itemSelectPage = GetComponentInChildren<ItemSelectPageUI>();
        itemSelectPage.SetButtonEvent(() => PlayerInventory.Instance.UseItem(itemSelectPage.CurrentItemId));
    }

    #region Currency UI
    public void UpdateCurrencyUI(Currency[] currencyDatas)
    {
        foreach (Currency currencyData in currencyDatas)
        {
            if (currencySlotUI.TryGetValue(currencyData.currencyType.ToString(), out CurrencySlotUI slot))
            {
                slot.UpdateUI(currencyData.icon, currencyData.Count);
            }
        }
    }
    #endregion

    #region Item UI
    public void UpdateItemUI(ItemData[] itemDatas)
    {
        itemDatas = SortItemDatas(itemDatas);

        //itemDatas 개수 만큼 SlotUI 활성화
        for (int i = 0; i < itemDatas.Length; i++)
        {
            if (i >= itemSlotUIs.Count)
            {
                itemSlotUIs.Add(CreateNewItemSlot());
            }

            //버튼 클릭 시 SelectPage 갱신 리스너 등록
            ItemData currentItem = itemDatas[i];
            itemSlotUIs[i].ActiveSlot(currentItem.icon, currentItem.count.ToString(), () =>
            {
                itemSelectPage.ActiveSelectPage(
                    currentItem.id,
                    currentItem.icon,
                    currentItem.itemName,
                    currentItem.count.ToString(),
                    currentItem.value.ToString(),
                    currentItem.description,
                    currentItem.type == ItemType.UseAble
                );
            });
        }

        for (int i = itemDatas.Length; i < itemSlotUIs.Count; i++)
        {
            itemSlotUIs[i].DesableSlot();
        }

        RefreshItemSelectPage();
    }

    private void RefreshItemSelectPage()
    {
        string selectedId = itemSelectPage.CurrentItemId;

        if (string.IsNullOrEmpty(selectedId))
        {
            itemSelectPage.DisableSelectPage();
            return;
        }

        ItemData currentSelected = PlayerInventory.Instance.GetItemData(selectedId);

        if (currentSelected == null || currentSelected.count <= 0)
        {
            itemSelectPage.DisableSelectPage();
            return;
        }

        itemSelectPage.ActiveSelectPage(
            currentSelected.id,
            currentSelected.icon,
            currentSelected.itemName,
            currentSelected.count.ToString(),
            currentSelected.value.ToString(),
            currentSelected.description,
            currentSelected.type == ItemType.UseAble
        );
    }

    public ItemSlotUI CreateNewItemSlot()
    {
        ItemSlotUI newSlot = Instantiate(itemSlotPrefab).GetComponent<ItemSlotUI>();
        newSlot.transform.SetParent(slotParent.gameObject.transform, false);
        return newSlot;
    }

    public void OnSortDropdownValueChanged(int index)
    {
        currentSortingType = (SortingType)index;
        if (PlayerInventory.Instance != null)
        {
            UpdateItemUI(PlayerInventory.Instance.ItemInventory);
        }
    }

    private ItemData[] SortItemDatas(ItemData[] itemDatas)
    {
        switch (currentSortingType)
        {
            case SortingType.TimeAscending:
                return itemDatas;
            case SortingType.TimeDescending:
                return itemDatas.Reverse().ToArray();
            case SortingType.NameAscending:
                return itemDatas.OrderBy(x => x.itemName).ToArray();
            case SortingType.NameDescending:
                return itemDatas.OrderByDescending(x => x.itemName).ToArray();
            case SortingType.StackAscending:
                return itemDatas.OrderBy(x => x.count).ToArray();
            case SortingType.StackDescending:
                return itemDatas.OrderByDescending(x => x.count).ToArray();
            default:
                return itemDatas;
        }
    }
    #endregion
}

public enum SortingType
{
    TimeAscending,
    TimeDescending,
    NameAscending,
    NameDescending,
    StackAscending,
    StackDescending,
}