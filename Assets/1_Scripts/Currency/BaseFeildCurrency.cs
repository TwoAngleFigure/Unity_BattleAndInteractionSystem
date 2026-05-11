using UnityEngine;

public class BaseFeildCurrency : MonoBehaviour
{
    public string currencyName;
    public int count;
    public LayerMask targetMask;

    public void Awake()
    {
        gameObject.name = currencyName;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsInTargetLayer(other.gameObject))
        {
            PlayerInventory.Instance.AddCurrency(currencyName, count);
            Destroy(gameObject);
        }
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & targetMask) != 0;
    }
}