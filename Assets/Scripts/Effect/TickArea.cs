using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TickArea : BaseArea
{
    [SerializeField] float _tickInterval = 1f;
    private Dictionary<GameObject, Coroutine> _tickCoroutines = new();

    public void OnTriggerEnter(Collider other)
    {
        if (IsInTargetLayer(other.gameObject))
        {
            if (!_tickCoroutines.ContainsKey(other.gameObject))
            {
                Coroutine routine = StartCoroutine(TickRoutine(other.gameObject));
                _tickCoroutines.Add(other.gameObject, routine);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (_tickCoroutines.TryGetValue(other.gameObject, out Coroutine routine))
        {
            StopCoroutine(routine);
            _tickCoroutines.Remove(other.gameObject);
        }
    }

    private IEnumerator TickRoutine(GameObject target)
    {
        while (target != null)
        {
            foreach (var e in effect)
            {
                if (e != null)
                    e.Effect(target);
            }
            yield return new WaitForSeconds(_tickInterval);
        }
    }

    private void OnDisable()
    {
        foreach (var routine in _tickCoroutines.Values)
        {
            StopCoroutine(routine);
        }
        _tickCoroutines.Clear();
    }
}
