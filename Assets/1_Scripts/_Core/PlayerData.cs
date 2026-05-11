using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "SO/Player Data")]
public class PlayerData : ScriptableObject
{
    public List<Currency> inventoryCurrencyList;
    public List<ItemData> inventoryItemList;
}
