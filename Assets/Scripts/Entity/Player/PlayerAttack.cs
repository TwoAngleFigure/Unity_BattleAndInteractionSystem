using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Option")]
    public WeaponObject weapon;
    public bool battleModState = false;
    [SerializeField] int _atk = 10;
    [SerializeField] float _atkSpeed = 1f;
    public bool canAttack = true;

    [Header("Motion Option")]
    [SerializeField] AnimationClip attackClip;
    public float _attackMotionTime;
    [SerializeField, Range(0, 100)] int _hitboxPreDelay = 10; // 백분율(%)
    [SerializeField, Range(0, 100)] int _hitboxDuration = 50; // 백분율(%)

    [Header("Input Action")]
    PlayerFieldControl _battleActions;
    InputAction _battleModToggleAction;
    InputAction _attackAction;

    [Header("Animation")]
    Animator _animator;
    private BasePlayer _player;

    #region initialize

    public void Init(BasePlayer player)
    {
        _player = player;
        _animator = player.Animator;
    }

    public void Awake()
    {
        if (weapon == null)
            weapon = GetComponentInChildren<WeaponObject>();

        _attackMotionTime = attackClip.length;
        SetAttackSpeed(_atkSpeed);
        InitWeapon();

        _battleActions = new();
        _battleModToggleAction = _battleActions.Player.BattleModChange;
        _attackAction = _battleActions.Player.Attack;
    }

    private void OnEnable()
    {
        _battleActions.Enable();
        _battleModToggleAction.performed += OnBattleModAction;
        _attackAction.performed += OnAttackAction;
    }

    private void OnDisable()
    {
        _battleModToggleAction.performed -= OnBattleModAction;
        _attackAction.performed -= OnAttackAction;
        _battleActions.Disable();
    }

    #endregion

    //모드 변경 로직
    public void OnBattleModAction(InputAction.CallbackContext callbackContext)
    {
        battleModState = !battleModState;
        weapon.gameObject.SetActive(battleModState);
        _animator.SetBool("isCombat", battleModState);
    }

    //공격 로직
    public void OnAttackAction(InputAction.CallbackContext callbackContext)
    {
        if (!battleModState || !canAttack || _player.State() != EntityState.Alive) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _player.ChangeState(EntityState.Attack);
        weapon.ActivateHitbox();

        _animator.SetTrigger("isAttack");

        yield return new WaitForSeconds(_attackMotionTime / _atkSpeed);

        if (_player.State() == EntityState.Attack)
        {
            _player.ChangeState(EntityState.Alive);
        }
    }

    public void SetWeapon(WeaponObject newWeapon)
    {
        weapon = newWeapon;
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
        canAttack = tri;
    }
}