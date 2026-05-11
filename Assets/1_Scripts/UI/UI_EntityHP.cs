using UnityEngine;
using UnityEngine.UI;

public class UI_EntityHP : MonoBehaviour
{
    [SerializeField] GameObject _targetEntity;
    [SerializeField] Slider _hpSlider;

    private void Awake()
    {
        _hpSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (_targetEntity != null && _targetEntity.TryGetComponent<BaseEntity>(out var entity))
        {
            entity.OnHpChanged += UpdateHpUI;
            UpdateHpUI(entity.Hp, entity.MaxHp);
        }
    }

    private void OnDestroy()
    {
        if (_targetEntity != null && _targetEntity.TryGetComponent<BaseEntity>(out var entity))
        {
            entity.OnHpChanged -= UpdateHpUI;
        }
    }

    private void UpdateHpUI(int current, int max)
    {
        if (_hpSlider != null)
        {
            _hpSlider.value = (float)current / max;
        }
    }
}