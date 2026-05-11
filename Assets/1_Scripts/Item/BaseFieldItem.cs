using UnityEngine;

public class BaseFieldItem : MonoBehaviour
{
    public ItemDataOrigin ItemDataTemplate;

    public ItemData ItemData;
    public LayerMask targetMask;

    public void Awake()
    {
        ItemData = new(
            ItemDataTemplate.id,
            ItemDataTemplate.itemName,
            ItemDataTemplate.description,
            ItemDataTemplate.value,
            1,
            ItemDataTemplate.itemType,
            ItemDataTemplate.icon,
            ItemDataTemplate.effects
            );
        gameObject.name = ItemData.itemName;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsInTargetLayer(other.gameObject))
        {
            PlayerInventory.Instance.AddItem(ItemData);
            Destroy(gameObject);
        }
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & targetMask) != 0;
    }
}