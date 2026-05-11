using UnityEngine;
using UnityEngine.UI;

public class ItemSlotShopUI : ItemSlotUI
{
    public Image icon;

    public Sprite goldIcon;
    public Sprite crystalIcon;

    public void SetCurrencyIcon(CurrencyType type)
    {
        if (icon != null)
        {
            if (type.Equals(CurrencyType.Gold))
                icon.sprite = goldIcon;
            else if (type.Equals(CurrencyType.Crystal))
                icon.sprite = crystalIcon;
        }
    }
}
