using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Image _icon;
    public TMP_Text _stackText;
    public Button button;
    public CanvasGroup canvasGroup;

    private bool _initialized = false;

    public void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;
        if (_icon == null)
            foreach (Image img in GetComponentsInChildren<Image>())
                if (img.gameObject != gameObject)
                    _icon =  img;
        if (_stackText == null) _stackText = GetComponentInChildren<TMP_Text>();
        if (button == null) button = GetComponentInChildren<Button>();
        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        _initialized = true;
    }

    public void ActiveSlot(Sprite icon, string text, UnityAction call)
    {
        EnsureInitialized();

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        _icon.sprite = icon;
        _stackText.text = text;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(call);
    }

    public void DesableSlot()
    {
        EnsureInitialized();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        button.onClick.RemoveAllListeners();
    }
}
