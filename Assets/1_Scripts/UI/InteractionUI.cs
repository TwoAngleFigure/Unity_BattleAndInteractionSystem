using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TMP_Text _text;

    public void Awake()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_text == null) _text = GetComponentInChildren<TMP_Text>();
        _canvasGroup.alpha = 0f;
    }

    public void SetActiveUI(bool active, string text = null)
    {
        if (active) _canvasGroup.alpha = 1f;
        else _canvasGroup.alpha = 0f;

        _text.text = text;
    }
}
