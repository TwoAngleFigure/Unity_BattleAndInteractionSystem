using UnityEngine;

public class StayArea : BaseArea
{
    public void OnTriggerEnter(Collider other)
    {
        if (IsInTargetLayer(other.gameObject))
        {
            foreach (var e in effect)
            {
                if (e != null)
                    e.Effect(other.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (IsInTargetLayer(other.gameObject))
        {
            foreach (var e in effect)
            {
                if (e != null)
                    e.Undo(other.gameObject);
            }
        }
    }
}
