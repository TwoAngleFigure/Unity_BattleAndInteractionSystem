using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseArea : MonoBehaviour
{
    public LayerMask targetMask;
    public List<EffectData> effectDataList = new List<EffectData>();
    protected List<BaseEffect> _effects = new List<BaseEffect>();

    public virtual void Awake()
    {
        foreach (var data in effectDataList)
        {
            var effect = EffectMapper.GetEffect(data);
            if (effect != null)
            {
                _effects.Add(effect);
            }
        }
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & targetMask) != 0;
    }
}
