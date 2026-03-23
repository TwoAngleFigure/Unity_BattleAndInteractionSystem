using UnityEngine;

public class HealingEffect : BaseEffect
{
    public int value;

    public override void Effect(GameObject target)
    {
        if(target.TryGetComponent<IHitable>(out var hitable))
        {
            hitable.Hp += value;
        }
    }
}