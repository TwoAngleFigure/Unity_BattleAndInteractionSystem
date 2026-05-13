using UnityEngine;
using UnityEngine.InputSystem;

public class TPS_TwoStepHitscanWeapon : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float aimRange = 100f;
    [SerializeField] private float shotRange = 100f;
    [SerializeField] private float muzzleBlockRadius = 0.1f;

    [SerializeField] private LayerMask aimMask;
    [SerializeField] private LayerMask shotMask;
    [SerializeField] private LayerMask muzzleBlockMask;

    [Header("Weapon")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float shotRadius = 0f;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Option")]
    [SerializeField] private bool checkMuzzleBlocked;

    private void OnRightClick(InputValue value)
    {
        if (value.isPressed == false) return;
        Fire();
    }

    private AimResult ResolveAimPoint()
    {
        Ray aimRay = aimCamera.ViewportPointToRay( new Vector3(0.5f, 0.5f, 0f));

        AimResult result = new AimResult
        {
            ray = aimRay,
            didHit = false,
            point = aimRay.GetPoint(aimRange)
        };

        if (Physics.Raycast(aimRay, out RaycastHit hit, aimRange, aimMask, QueryTriggerInteraction.Ignore))
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
            ? Mathf.Min(shotRange, distanceToAimPoint + 0.05f)
            : shotRange;

        ShotResult result = new ShotResult
        {
            origin = muzzle.position,
            direction = shotDirection,
            distance = castDistance,
            didHit = false
        };

        if (CastShot(muzzle.position, shotDirection,castDistance, out RaycastHit shotHit))
        {
            result.didHit = true;
            result.hit = shotHit;
        }

        return result;
    }

    private bool CastShot(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        if (shotRadius > 0f)
        {
            return Physics.SphereCast(
                origin,
                shotRadius,
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
            muzzleBlockRadius,
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
            damageable.OnHit(damage);
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab,
                hit.point,
                Quaternion.LookRotation(hit.normal));
        }
    }

    private void DrawDebugRays(AimResult aim, ShotResult shot)
    {
        float aimDistance = aim.didHit ? aim.hit.distance : aimRange;
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
        if (muzzle != null)
            Gizmos.DrawWireSphere(muzzle.position, muzzleBlockRadius);
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
