using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseArea : MonoBehaviour
{
    public LayerMask targetMask;
    [SerializeField] protected List<BaseEffect> effect;

    public virtual void Awake()
    {
        effect = GetComponents<BaseEffect>().ToList();
    }

    protected bool IsInTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & targetMask) != 0;
    }
}
