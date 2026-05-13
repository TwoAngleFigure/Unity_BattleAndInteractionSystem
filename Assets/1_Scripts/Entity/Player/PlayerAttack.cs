using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Option")]
    public WeaponTrigger _weaponTrigger;
    public bool battleModState = false;
    [SerializeField] int _atk = 10;
    [SerializeField] float _atkSpeed = 1f;
    public bool canAttack = true;

    [Header("Motion Option")]
    [SerializeField] AnimationClip attackClip;
    public float _attackMotionTime;
    [SerializeField, Range(0, 100)] int _hitboxPreDelay = 10;
    [SerializeField, Range(0, 100)] int _hitboxDuration = 50;
    private float calculatedPreDelay;
    private float calculatedDuration;

    [Header("Animation")]
    [SerializeField] Animator _animator;
    private BasePlayer _player;

    #region initialize

    public void Init(BasePlayer player)
    {
        _player = player;
        _animator = player.Animator;

        if (_weaponTrigger == null)
            _weaponTrigger = GetComponentInChildren<WeaponTrigger>();

        _attackMotionTime = attackClip.length;
        SetAttackSpeed(_atkSpeed);
        InitWeapon();
    }

    #endregion

    #region PlayerInput Send Messages

    private void OnBattleModChange(InputValue value)
    {
        if (value.isPressed == false) return;
        battleModState = !battleModState;
        _animator.SetBool("isCombat", battleModState);
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed == false) return;
        if (battleModState == false || canAttack == false || _player.State() != EntityState.Alive) return;
        StartCoroutine(AttackRoutine());
        StartCoroutine(ActiveTriggerCoroutine());
    }

    #endregion

    private IEnumerator AttackRoutine()
    {
        _player.ChangeState(EntityState.Attack);
        _animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(_attackMotionTime / _atkSpeed);
        if (_player.State() == EntityState.Attack)
            _player.ChangeState(EntityState.Alive);
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
        canAttack = tri;
    }
}