using UnityEngine;
using System.Collections;

public class BasePlayer : BaseEntity
{
    [Header("Sub Systems")]
    private PlayerMovement _movement;
    private PlayerAttack _attack;
    private PlayerInventory _inventory;

    public AnimationClip hitMotion;
    float _hitMotionTime;

    public static BasePlayer Instance { get; private set; }

    public override void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        base.Awake();

        if (_movement == null) _movement = GetComponent<PlayerMovement>();
        if (_attack == null) _attack = GetComponent<PlayerAttack>();

        _movement.Init(this);
        _attack.Init(this);

        _hitMotionTime = hitMotion.length;
    }

    private void Start()
    {
        _inventory = PlayerInventory.Instance;
        if (_inventory == null)
        {
            _inventory = GetComponent<PlayerInventory>();
        }

        if (_inventory != null)
        {
            _inventory.OnInventoryToggled += HandleInventoryToggled;
        }
    }

    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnInventoryToggled -= HandleInventoryToggled;
        }
    }

    private void HandleInventoryToggled(bool isInventoryOpen)
    {
        UpdateActionState();
    }

    protected override void OnStateChanged(EntityState oldState, EntityState newState)
    {
        UpdateActionState();
    }

    private void UpdateActionState()
    {
        bool canAct = (State() == EntityState.Alive);

        if (_inventory != null && _inventory.InventoryToggleState)
        {
            canAct = false;
        }

        if (_movement != null) _movement.SetCanMove(canAct);
        if (_attack != null) _attack.SetCanAttack(canAct);
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
