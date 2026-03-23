using UnityEngine;
using System;

public class BaseEntity : MonoBehaviour, IHitable
{
    public Action<int, int> OnHpChanged;

    public EntityState State() => _state;
    [SerializeField] protected EntityState _state = EntityState.Alive;

    public void ChangeState(EntityState newState)
    {
        if (_state == newState) return;
        if (_state == EntityState.Dead) return;

        EntityState oldState = _state;
        _state = newState;
        OnStateChanged(oldState, newState);
    }

    protected virtual void OnStateChanged(EntityState oldState, EntityState newState) { }

    public int MaxHp => _maxHp;
    [SerializeField] int _maxHp;

    public int Hp
    {
        get => _hp;
        set
        {
            _hp = Mathf.Clamp(value, 0, _maxHp);
            OnHpChanged?.Invoke(_hp, _maxHp);

            if (_hp <= 0 && _state != EntityState.Dead)
            {
                OnDead();
            }
        }
    }
    [SerializeField] int _hp;

    protected Animator _animator;
    public Animator Animator => _animator;

    public virtual void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public virtual void OnHit(int damage)
    {
        _animator.SetTrigger("HitTri");
        Hp -= damage;
    }

    public virtual void OnDead()
    {
        _state = EntityState.Dead;
        OnStateChanged(EntityState.Alive, EntityState.Dead);
        _animator.SetBool("isDead", true);
        _animator.SetTrigger("DeadTri");
    }
}