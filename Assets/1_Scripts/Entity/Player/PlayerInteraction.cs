using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Action")]
    private PlayerFieldControl _inputActions;
    private InputAction _interactionAction;

    [Header("Interaction")]
    private IInteractive _interactiveTarget;
    public GameObject target;
    public InteractionUI interactionUI;

    public void Awake()
    {
        _inputActions = new();
        _interactionAction = _inputActions.Player.Interaction;
    }

    public void OnEnable()
    {
        _inputActions.Enable();
        _interactionAction.performed += OnInteractiveAction;
    }

    public void OnDisable()
    {
        _inputActions.Disable();
        _interactionAction.performed -= OnInteractiveAction;
    }

    public void OnInteractiveAction(InputAction.CallbackContext callbackContext)
    {
        if (_interactiveTarget != null)
        {
            _interactiveTarget.Interaction();
            interactionUI.SetActiveUI(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractive>(out IInteractive target))
        {
            _interactiveTarget = target;
            this.target = other.gameObject;
            interactionUI.SetActiveUI(true, target.InteractionType());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            if (_interactiveTarget != null)
            {
                _interactiveTarget.Clear();
                this.target = null;
                _interactiveTarget = null;
                interactionUI.SetActiveUI(false);
            }
        }
    }
}
