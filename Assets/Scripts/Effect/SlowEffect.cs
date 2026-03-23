using UnityEngine;
using System.Collections.Generic;

public class SlowEffect : BaseEffect
{
    public float value;
    private Dictionary<GameObject, float> _originValues = new();

    public override void Effect(GameObject target)
    {
        if(target.TryGetComponent<BaseEnemy>(out var enemy))
        {
            if (!_originValues.ContainsKey(target))
                _originValues.Add(target, enemy.MaxSpeed);

            enemy.MaxSpeed -= value;
        }
    }

    public override void Undo(GameObject target)
    {
        if(target.TryGetComponent<BaseEnemy>(out var enemy))
        {
            if (_originValues.TryGetValue(target, out float origin))
            {
                enemy.MaxSpeed = origin;
                _originValues.Remove(target);
            }
        }
    }
}