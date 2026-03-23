using UnityEngine;
using System.Collections;

public class BasePlayer : BaseEntity
{
    [Header("Sub Systems")]
    private PlayerMovement _movement;
    private PlayerAttack _attack;

    public AnimationClip hitMotion;
    float _hitMotionTime;

    public override void Awake()
    {
        base.Awake();

        if (_movement == null) _movement = GetComponent<PlayerMovement>();
        if (_attack == null) _attack = GetComponent<PlayerAttack>();

        _movement.Init(this);
        _attack.Init(this);

        _hitMotionTime = hitMotion.length;
    }

    protected override void OnStateChanged(EntityState oldState, EntityState newState)
    {
        bool canAct = (newState == EntityState.Alive);
        _movement.SetCanMove(canAct);
        _attack.SetCanAttack(canAct);
    }

    public override void OnHit(int damage)
    {
        if (State() == EntityState.Dead) return;

        base.OnHit(damage);
        StartCoroutine(HitStateRoutine());
    }

    private IEnumerator HitStateRoutine()
    {
        ChangeState(EntityState.Hit);

        yield return new WaitForSeconds(_hitMotionTime);

        if (State() == EntityState.Hit)
        {
            ChangeState(EntityState.Alive);
        }
    }

    public void SetCanMove(bool tri) => _movement.SetCanMove(tri);
}
