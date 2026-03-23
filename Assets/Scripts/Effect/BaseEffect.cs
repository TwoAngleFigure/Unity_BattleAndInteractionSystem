using UnityEngine;

public abstract class BaseEffect : MonoBehaviour
{
    public abstract void Effect(GameObject target);
    public virtual void Undo(GameObject target) { }
}
