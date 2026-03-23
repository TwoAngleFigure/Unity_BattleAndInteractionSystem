using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Option")]
    public WeaponObject weapon;
    [SerializeField] int _atk = 5;
    [SerializeField] float _atkSpeed = 1f;
    bool _canAttack = true;

    [Header("Motion Option")]
    [SerializeField] AnimationClip attackClip;
    [SerializeField] float _attackMotionTime;
    [SerializeField, Range(0, 100)] int _hitboxPreDelay = 20;
    [SerializeField, Range(0, 100)] int _hitboxDuration = 40;

    [Header("Animation")]
    [SerializeField] Animator _animator;

    BaseEnemy _baseEnemy;

    public void Init(BaseEnemy baseEnemy)
    {
        _baseEnemy = baseEnemy;
        _animator = _baseEnemy.Animator;

        if (weapon == null)
            weapon = _baseEnemy.GetComponentInChildren<WeaponObject>();

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
        }
    }

    private IEnumerator AttackRoutine()
    {
        _baseEnemy.ChangeState(EntityState.Attack);
        weapon.ActivateHitbox();

        _animator.SetTrigger("AttackTri");

        // 코루틴 타이머만으로 상태 복구 (AnimationCall 콜백 제거)
        yield return new WaitForSeconds(_attackMotionTime / _atkSpeed);

        if (_baseEnemy.State() == EntityState.Attack)
        {
            _baseEnemy.ChangeState(EntityState.Alive);
        }
    }

    public void SetAttackSpeed(float speed)
    {
        _atkSpeed = speed;
        _animator.SetFloat("AttackSpeed", _atkSpeed);
        InitWeapon();
    }

    public void InitWeapon()
    {
        if (weapon == null) return;

        float actualMotionTime = _attackMotionTime / _atkSpeed;

        float calculatedPreDelay = actualMotionTime * (_hitboxPreDelay / 100f);
        float calculatedDuration = actualMotionTime * (_hitboxDuration / 100f);

        weapon.Init(_atk, calculatedPreDelay, calculatedDuration);
    }

    public void SetCanAttack(bool tri)
    {
        _canAttack = tri;
    }
}
