using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSelectPageUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemStack;
    public TMP_Text itemDescription;
    public Button useButton;

    private string currentItemId;

    public string CurrentItemId => currentItemId;

    public void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (useButton == null) useButton = GetComponentInChildren<Button>();
    }

    public void ActiveSelectPage(string itemId, Sprite icon, string nameText, string countText, string priceText,string descText, bool showUseBtn)
    {
        currentItemId = itemId;

        if (itemIcon != null) itemIcon.sprite = icon;
        if (itemName != null) itemName.text = nameText;
        if (itemStack != null && itemStack.CompareTag("Stack")) itemStack.text = countText;
        if (itemStack != null && itemStack.CompareTag("Price")) itemStack.text = priceText;
        if (itemDescription != null) itemDescription.text = descText;
        if (useButton != null) useButton.gameObject.SetActive(showUseBtn);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
        }
    }

    public void DisableSelectPage()
    {
        currentItemId = string.Empty;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }
    }

    public void SetButtonEvent(UnityAction call)
    {
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(call);
        }
    }
}
