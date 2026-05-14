using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackSystem : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponData_Base[] weapons = new WeaponData_Base[3];
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private WeaponData_Base currentWeapon;

    [Header("Option")]
    [SerializeField] private WeaponTrigger weaponTrigger;
    [SerializeField] private bool battleModState = false;
    [SerializeField] private bool canAttack = true;

    [Header("Attack Speed Option")]
    [SerializeField] private bool useAttackSpeed;
    [SerializeField] private float atkSpeed = 1f;

    [Header("Motion Option")]
    private float _attackMotionTime;
    private float calculatedPreDelay;
    private float calculatedDuration;

    [Header("Animation")]
    private Animator _animator;
    private BasePlayer _player;

    #region initialize

    public void Init(BasePlayer player)
    {
        _player = player;
        _animator = player.Animator;

        if (weaponTrigger == null)
            weaponTrigger = GetComponentInChildren<WeaponTrigger>();

        SwapCurrentWeaponForIndex(0);
    }

    #endregion

    #region PlayerInput Send Messages

    private void OnBattleModChange(InputValue value)
    {
        if (value.isPressed == false) return;
        battleModState = !battleModState;
        _animator.SetBool("isCombat", battleModState);
    }

    private void OnSelectWeapon(InputValue value)
    {
        int index;

        if (Keyboard.current.digit1Key.isPressed) index = 0;
        else if (Keyboard.current.digit2Key.isPressed) index = 1;
        else if (Keyboard.current.digit3Key.isPressed) index = 2;
        else return;

        currentWeaponIndex = index;
        SwapCurrentWeaponForIndex(currentWeaponIndex);
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed == false) return;
        if (battleModState == false || canAttack == false || _player.State() != EntityState.Alive) return;

        switch (currentWeapon.weaponRangeType)
        {
            case WeaponRangeType.Short:
                StartCoroutine(AttackRoutine());
                StartCoroutine(ActiveShortAttackTriggerCoroutine());
                break;

            case WeaponRangeType.Long:
                StartCoroutine(ActiveLongAttackFireCoroutine());
                break;
        }
    }

    private void OnRightClick(InputValue value)
    {

    }
    #endregion

    private IEnumerator AttackRoutine()
    {
        _player.ChangeState(EntityState.Attack);
        yield return new WaitForSeconds(_attackMotionTime / atkSpeed);
        if (_player.State() == EntityState.Attack)
            _player.ChangeState(EntityState.Alive);
    }

    private IEnumerator ActiveLongAttackFireCoroutine()
    {
        _animator.SetTrigger("isLongRangeAttack");
        yield return new WaitForSeconds(calculatedPreDelay);
        Fire();
        yield return new WaitForSeconds(calculatedDuration);
    }

    #region Short Attack

    public void SwapCurrentWeaponForIndex(int index)
    {
        currentWeapon = weapons[index];

        CaculateAttackSpeed(currentWeapon);

        weaponTrigger.SetWeapon(currentWeapon.damage);
    }

    public void SetAttackSpeed(float speed, WeaponData_Base wData)
    {
        atkSpeed = speed;
        _animator.SetFloat("AttackSpeed", atkSpeed);
        CaculateAttackSpeed(wData);
    }

    private void CaculateAttackSpeed(WeaponData_Base wData)
    {
        _attackMotionTime = wData.attackMotionClip.length;

        float actualMotionTime = _attackMotionTime / atkSpeed;
        calculatedPreDelay = actualMotionTime * (wData._hitboxPreDelay / 100f);
        calculatedDuration = actualMotionTime * (wData._hitboxDuration / 100f);
    }

    public void SetCanAttack(bool tri)
    {
        canAttack = tri;
    }

    private IEnumerator ActiveShortAttackTriggerCoroutine()
    {
        _animator.SetTrigger("isAttack");
        yield return new WaitForSeconds(calculatedPreDelay);
        weaponTrigger.gameObject.SetActive(true);
        yield return new WaitForSeconds(calculatedDuration);
        weaponTrigger.gameObject.SetActive(false);
    }

    #endregion

    #region Long Attack

    [Header("Aim")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform muzzle;

    [SerializeField] private LayerMask aimMask;
    [SerializeField] private LayerMask shotMask;
    [SerializeField] private LayerMask muzzleBlockMask;

    [Header("Option")]
    [SerializeField] private bool checkMuzzleBlocked;

    private AimResult ResolveAimPoint()
    {
        Ray aimRay = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        AimResult result = new AimResult
        {
            ray = aimRay,
            didHit = false,
            point = aimRay.GetPoint(currentWeapon.aimRange)
        };

        if (Physics.Raycast(aimRay, out RaycastHit hit, currentWeapon.aimRange, aimMask, QueryTriggerInteraction.Ignore))
        {
            result.didHit = true;
            result.hit = hit;
            result.point = hit.point;
        }

        return result;
    }

    private ShotResult FireFromMuzzle(AimResult aimResult)
    {
        Vector3 toAimPoint = aimResult.point - muzzle.position;

        if (toAimPoint.sqrMagnitude < 0.0001f)
        {
            toAimPoint = aimCamera.transform.forward;
        }

        Vector3 shotDirection = toAimPoint.normalized;
        float distanceToAimPoint = toAimPoint.magnitude;

        float castDistance = aimResult.didHit
            ? Mathf.Min(currentWeapon.shotRange, distanceToAimPoint + 0.05f)
            : currentWeapon.shotRange;

        ShotResult result = new ShotResult
        {
            origin = muzzle.position,
            direction = shotDirection,
            distance = castDistance,
            didHit = false
        };

        if (CastShot(muzzle.position, shotDirection, castDistance, out RaycastHit shotHit))
        {
            result.didHit = true;
            result.hit = shotHit;
        }

        return result;
    }

    private bool CastShot(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        if (currentWeapon.shotRadius > 0f)
        {
            return Physics.SphereCast(
                origin,
                currentWeapon.shotRadius,
                direction,
                out hit,
                distance,
                shotMask,
                QueryTriggerInteraction.Ignore);
        }

        return Physics.Raycast(
            origin,
            direction,
            out hit,
            distance,
            shotMask,
            QueryTriggerInteraction.Ignore);
    }

    private bool IsMuzzleBlocked()
    {
        if (checkMuzzleBlocked == false)
        {
            return false;
        }

        return Physics.CheckSphere(
            muzzle.position,
            currentWeapon.muzzleBlockRadius,
            muzzleBlockMask,
            QueryTriggerInteraction.Ignore);
    }

    private void HandleHit(RaycastHit hit, AimResult aimResult)
    {
        string aimName = aimResult.didHit
            ? aimResult.hit.collider.name
            : "없음";

        string shotName = hit.collider.name;
        Debug.Log($"카메라 조준: {aimName} / 실제 피격: {shotName}");

        IHitable damageable =
            hit.collider.GetComponentInParent<IHitable>();

        if (damageable != null)
        {
            damageable.OnHit(currentWeapon.damage);
        }

        if (currentWeapon.hitEffectPrefab != null)
        {
            Instantiate(currentWeapon.hitEffectPrefab,
                hit.point,
                Quaternion.LookRotation(hit.normal));
        }
    }

    private void DrawDebugRays(AimResult aim, ShotResult shot)
    {
        float aimDistance = aim.didHit ? aim.hit.distance : currentWeapon.aimRange;
        Debug.DrawRay(
            aim.ray.origin,
            aim.ray.direction * aimDistance,
            Color.cyan,
            0.5f);

        float shotDistance = shot.didHit ? shot.hit.distance : shot.distance;
        Debug.DrawRay(
            shot.origin,
            shot.direction * shotDistance,
            shot.didHit ? Color.red : Color.yellow,
            0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (muzzle != null && currentWeapon != null)
            Gizmos.DrawWireSphere(muzzle.position, currentWeapon.muzzleBlockRadius);
    }

    public void Fire()
    {
        if (aimCamera == null || muzzle == null)
        {
            Debug.LogWarning("Aim Camera 또는 Muzzle이 없습니다.");
            return;
        }

        if (IsMuzzleBlocked())
        {
            Debug.Log("발사 불가: 총구가 장애물에 너무 가깝습니다.");
            return;
        }

        AimResult aimResult = ResolveAimPoint();
        ShotResult shotResult = FireFromMuzzle(aimResult);

        DrawDebugRays(aimResult, shotResult);

        if (shotResult.didHit)
        {
            HandleHit(shotResult.hit, aimResult);
        }
    }
    #endregion
}


public struct AimResult
{
    public Ray ray;
    public bool didHit;
    public Vector3 point;
    public RaycastHit hit;
}

public struct ShotResult
{
    public Vector3 origin;
    public Vector3 direction;
    public float distance;
    public bool didHit;
    public RaycastHit hit;
}
