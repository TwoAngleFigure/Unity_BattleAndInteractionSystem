using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string id;
    public string itemName;
    public string description;
    public int value; //가치
    public int count; //개수
    public ItemType type;

    public Sprite icon;
    public EffectData[] effects;

    public ItemData(string id, string itemName, string description, int value, int count, ItemType type, Sprite icon, EffectData[] effects)
    {
        this.id = id;
        this.itemName = itemName;
        this.description = description;
        this.value = value;
        this.count = count;
        this.type = type;
        this.icon = icon;
        this.effects = effects;
    }

    public ItemData(ItemDataOrigin dataOrigin)
    {
        this.id = dataOrigin.id;
        this.itemName = dataOrigin.itemName;
        this.description = dataOrigin.description;
        this.value = dataOrigin.value;
        this.count = dataOrigin.count;
        this.type = dataOrigin.itemType;
        this.icon = dataOrigin.icon;
        this.effects = dataOrigin.effects;
    }

    public ItemData Clone(int overrideCount = -1)
    {
        return new ItemData(
            this.id,
            this.itemName,
            this.description,
            this.value,
            overrideCount == -1 ? this.count : overrideCount,
            this.type,
            this.icon,
            this.effects
        );
    }

    public void Use(GameObject target)
    {
        if (effects == null) return;
        foreach (var data in effects)
        {
            var effect = EffectMapper.GetEffect(data);
            if (effect != null)
            {
                effect.Effect(target);
            }
        }
    }
}
