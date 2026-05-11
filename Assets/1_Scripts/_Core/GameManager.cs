using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData;

    public PlayerInventory playerInventory;

    public void Awake()
    {
        //Singlton
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

        ///Initialize 
        //Inventory
        if(playerInventory ==null) { playerInventory = FindFirstObjectByType<PlayerInventory>(); }
        playerInventory.Initailize();
        LoadInventory();
    }

    #region Save & Load
    public void SaveInventory()
    {
        playerData.inventoryCurrencyList = playerInventory.CurrencyInventory.ToList();
        playerData.inventoryItemList = playerInventory.ItemInventory.ToList();
    }

    public void LoadInventory()
    {
        foreach(var item in playerData.inventoryCurrencyList)
        {
            playerInventory.SetCurrencyData(item);
        }
        foreach(var item in playerData.inventoryItemList)
        {
            playerInventory.AddItem(item);
        }
    }
    #endregion
}
