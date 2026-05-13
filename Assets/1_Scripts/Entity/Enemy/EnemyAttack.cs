using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Option")]
    public WeaponTrigger _weaponTrigger;
    [SerializeField] int _atk = 5;
    [SerializeField] float _atkSpeed = 1f;
    bool _canAttack = true;

    [Header("Motion Option")]
    [SerializeField] AnimationClip attackClip;
    [SerializeField] float _attackMotionTime;
    [SerializeField, Range(0, 100)] int _hitboxPreDelay = 20;
    [SerializeField, Range(0, 100)] int _hitboxDuration = 40;
    float calculatedPreDelay;
    float calculatedDuration;

    [Header("Animation")]
    [SerializeField] Animator _animator;

    BaseEnemy _baseEnemy;

    public void Init(BaseEnemy baseEnemy)
    {
        _baseEnemy = baseEnemy;
        _animator = _baseEnemy.Animator;

        if (_weaponTrigger == null)
            _weaponTrigger = _baseEnemy.GetComponentInChildren<WeaponTrigger>();

        if (attackClip != null)
            _attackMotionTime = attackClip.length;

        SetAttackSpeed(_atkSpeed);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IHitable>(out var player) && _canAttack && _baseEnemy.State() == EntityState.Alive)
        {
            if (player.State() == EntityState.Dead) return;
            StartCoroutine(AttackRoutine());
            StartCoroutine(ActiveTriggerCoroutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        _baseEnemy.ChangeState(EntityState.Attack);

        _animator.SetTrigger("AttackTri");

        yield return new WaitForSeconds(_attackMotionTime / _atkSpeed);

        if (_baseEnemy.State() == EntityState.Attack)
        {
            _baseEnemy.ChangeState(EntityState.Alive);
        }
    }

    private IEnumerator ActiveTriggerCoroutine()
    {
        yield return new WaitForSeconds(calculatedPreDelay);

        _weaponTrigger.gameObject.SetActive(true);

        yield return new WaitForSeconds(calculatedDuration);

        _weaponTrigger.gameObject.SetActive(false);
    }

    public void SetAttackSpeed(float speed)
    {
        _atkSpeed = speed;
        _animator.SetFloat("AttackSpeed", _atkSpeed);
        InitWeapon();
    }

    public void InitWeapon()
    {
        if (_weaponTrigger == null) return;

        float actualMotionTime = _attackMotionTime / _atkSpeed;

        calculatedPreDelay = actualMotionTime * (_hitboxPreDelay / 100f);
        calculatedDuration = actualMotionTime * (_hitboxDuration / 100f);

        _weaponTrigger.Initialize(_atk);
    }

    public void SetCanAttack(bool tri)
    {
        _canAttack = tri;
    }
}
