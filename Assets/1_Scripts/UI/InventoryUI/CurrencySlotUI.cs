using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencySlotUI : MonoBehaviour
{
    [SerializeField] private string _currencyId;
    public Image _icon;
    public TMP_Text _currencyStackText;

    [Header("Getter")]
    public string currencyId { get => _currencyId; }

    public void Awake()
    {
        if(_icon == null) _icon = GetComponentInChildren<Image>();
        if(_currencyStackText == null) _currencyStackText = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateUI(Sprite icon, int currentStack)
    {
        this._icon.sprite = icon;
        _currencyStackText.text = currentStack.ToString();
    }
}
