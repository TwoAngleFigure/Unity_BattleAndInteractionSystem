using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] int _atk;
    [SerializeField] float _hitboxPreDelay;
    [SerializeField] float _hitboxDuration;
    public bool tri;

    private HashSet<GameObject> _hitObjects = new();

    public void Init(int atk, float preDelay, float duration)
    {
        _atk = atk;
        _hitboxPreDelay = preDelay;
        _hitboxDuration = duration;
    }

    public void ResetHitList()
    {
        _hitObjects.Clear();
    }

    public void ActivateHitbox()
    {
        StartCoroutine(HitboxRoutine());
    }

    private IEnumerator HitboxRoutine()
    {
        ResetHitList();
        tri = false; 
        yield return new WaitForSeconds(_hitboxPreDelay);
        tri = true;
        yield return new WaitForSeconds(_hitboxDuration);
        tri = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (tri)
        {
            // LayerMask 필터링 추가
            // (1 << targetLayer) & targetMask != 0
            if (((1 << other.gameObject.layer) & _targetLayer) != 0)
            {
                if (other.TryGetComponent(out IHitable hitable))
                {
                    if (_hitObjects.Add(other.gameObject)&&hitable.State() != EntityState.Dead)
                    {
                        hitable.OnHit(_atk);
                        Debug.Log($"Hit Success: {other.name}, Damage: {_atk}");
                    }
                }
            }
        }
    }
}
