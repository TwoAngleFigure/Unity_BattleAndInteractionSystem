using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseItem : MonoBehaviour
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

    public void OnTriggerEnter(Collider other)
    {
        if(IsInTargetLayer(other.gameObject))
            foreach(BaseEffect effect in effect)
            {
                effect.Effect(other.gameObject);
            }
    }
}
