using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    private GameObject _hitTarget;
    private IInteractive _interactiveTarget;
    public InteractionUI interactionUI;

    [Header("Raycast")]
    [SerializeField] private Camera m_cam;
    [SerializeField] private LayerMask m_hittableMask;
    [SerializeField] private float m_maxDistance;

    public void Awake()
    {
        if (m_cam == null) m_cam = Camera.main;
    }

    #region PlayerInput Send Messages

    private void OnInteraction(InputValue value)
    {
        if (value.isPressed == false) return;

        if (_interactiveTarget != null)
        {
            _interactiveTarget.Interaction();
            interactionUI.SetActiveUI(false);
        }
    }

    #endregion

    public void Update()
    {
        DetectInteractionTarget();
    }

    public void DetectInteractionTarget()
    {
        Vector2 _screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray _ray = m_cam.ScreenPointToRay(_screenCenter);

        if (Physics.Raycast(_ray, out var hit, m_maxDistance, m_hittableMask))
        {
            Debug.DrawLine(_ray.origin, hit.point, Color.green);
            if (hit.collider.gameObject == _hitTarget) return;
            _hitTarget = hit.collider.gameObject;

            if (hit.collider.TryGetComponent<IInteractive>(out var interactionTarget))
            {
                _interactiveTarget = interactionTarget;
                interactionUI.SetActiveUI(true, _interactiveTarget.InteractionType());
            }
        }
        else
        {
            Debug.DrawLine(_ray.origin, _ray.direction * m_maxDistance, Color.red);
            if (_interactiveTarget != null) _interactiveTarget = null;
            if (_hitTarget != null) _hitTarget = null;
            interactionUI.SetActiveUI(false);
        }
    }
}
