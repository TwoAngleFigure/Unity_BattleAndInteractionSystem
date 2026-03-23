using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseEntity
{
    [Header("Reference")]
    [SerializeField] EnemyAttack _enemyAttack;

    [Header("Move Option")]
    public GameObject target;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] float _moveAcceleration = 50f;
    public float MoveAcceleration => _moveAcceleration;
    [SerializeField] float _maxSpeed = 2f;
    public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = value; }
    [SerializeField] float _rotationSpeed = 720f;
    [SerializeField] float _stoppingDistance = 1.5f;
    [SerializeField] bool canMove = true;

    [Header("Hit Option")]
    [SerializeField] float _hitMotionTime = 0.5f;

    public void SetCanMove(bool tri) { canMove = tri; }

    public new void Awake()
    {
        base.Awake();
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        if (_enemyAttack != null)
            _enemyAttack = GetComponentInChildren<EnemyAttack>();
        _enemyAttack.Init(this);
    }

    protected override void OnStateChanged(EntityState oldState, EntityState newState)
    {
        bool canAct = (newState == EntityState.Alive);
        SetCanMove(canAct);
        _enemyAttack.SetCanAttack(canAct);
    }

    public void FixedUpdate()
    {
        if (target == null || canMove == false || State() == EntityState.Dead) return;
        if (target.TryGetComponent<IHitable>(out var player))
        {
            if (player.State() == EntityState.Dead)
            {
                target = null;
                return;
            }
        }
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }

        float currentSpeed = _rigidbody.linearVelocity.magnitude;

        if (_animator != null)
        {
            _animator.SetFloat("Speed", currentSpeed);
        }

        if (distance > _stoppingDistance && currentSpeed < _maxSpeed)
        {
            _rigidbody.AddForce(direction.normalized * _moveAcceleration, ForceMode.Acceleration);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            target = other.gameObject;
        }
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
}
