using System;
using UnityEngine;

[Serializable]
public abstract class BaseEffect
{
    public abstract void Effect(GameObject target);
    public virtual void Undo(GameObject target) { }
}
