using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "SO/Currency")]
public class Currency : ScriptableObject
{
    public CurrencyType currencyType;
    public string description;
    [SerializeField] private int _count; //°³¼ö

    public Sprite icon;

    [Header("Getter")]
    public int Count { get => _count; }

    public void AddCount(int num)
    {
        _count += num;
    }

    public bool RemoveCount(int num) 
    {
        int result = _count - num;
        if(result < 0)
            return false;
        else
            _count -= num;
        return true;
    }
}

public enum CurrencyType
{
    Gold,
    Crystal,
}