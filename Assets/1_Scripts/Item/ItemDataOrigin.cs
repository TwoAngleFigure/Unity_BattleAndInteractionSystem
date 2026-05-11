using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "SO/Item Data")]
public class ItemDataOrigin : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;

    public ItemType itemType;
    public int value; //가치
    public int count; //개수

    public GameObject prefab;
    public Sprite icon;

    public EffectData[] effects;
}

public enum ItemType
{
    Default,
    UseAble
}