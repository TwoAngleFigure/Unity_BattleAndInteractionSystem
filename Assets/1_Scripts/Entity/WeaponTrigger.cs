using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] Vector3 _center;
    [SerializeField] Vector3 hitboxSize = new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField] int _atk;

    private HashSet<GameObject> _hitObjects = new();

    public void SetWeapon(int dmg)
    {
        transform.localPosition = _center;
        _atk = dmg;
    }

    private void OnEnable()
    {
        _hitObjects.Clear();
    }

    private void Update()
    {
        Overlap();
    }

    void Overlap()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, hitboxSize, quaternion.identity, _targetLayer); ;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IHitable hitable))
            {
                if (_hitObjects.Add(hitCollider.gameObject) && hitable.State() != EntityState.Dead)
                {
                    hitable.OnHit(_atk);
                    Debug.Log($"Hit Success: {hitCollider.name}, Damage: {_atk}");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(transform.position, hitboxSize);
    }
}
