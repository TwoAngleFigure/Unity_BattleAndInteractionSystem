using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _cameraTransform;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private float _walkMaxSpeed = 2f;
    [SerializeField] private float _sprintMaxSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 10f;

    [Header("Gravity")]
    [SerializeField] private float _gravity = -20f;

    private Vector2 _moveInput;
    private bool _isSprinting;
    private Vector3 _velocity;
    private bool _wasGrounded;

    private BasePlayer _player;

    public void Init(BasePlayer player)
    {
        _player = player;
        _animator = player.Animator;
    }

    private void Awake()
    {
        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        if (_controller == null)
            _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool isGrounded = _controller.isGrounded;

        // 착지 감지
        if (isGrounded && _wasGrounded == false)
        {
            _animator.SetBool("isAir", false);
            if (_player != null && _player.State() == EntityState.Air)
                _player.ChangeState(EntityState.Alive);
        }

        // 바닥에 있을 때 하방 속도 리셋
        if (isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        float currentMaxSpeed = _isSprinting ? _sprintMaxSpeed : _walkMaxSpeed;
        Vector3 horizontalMove = Vector3.zero;

        if (_canMove)
        {
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * _moveInput.y) + (camRight * _moveInput.x);

            if (moveDir.sqrMagnitude > 0.001f)
            {
                moveDir = Vector3.ClampMagnitude(moveDir, 1f);

                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                horizontalMove = moveDir * currentMaxSpeed;
            }
        }

        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;

        // 이동 + 중력을 단일 Move()로 합산
        Vector3 finalMove = (horizontalMove + _velocity) * Time.deltaTime;
        _controller.Move(finalMove);

        // 애니메이션 속도
        _animator.SetFloat("Speed", horizontalMove.magnitude);

        _wasGrounded = isGrounded;
    }

    #region PlayerInput Send Messages

    private void OnMovement(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void OnSprint(InputValue value)
    {
        _isSprinting = value.isPressed;
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed == false) return;

        if (_controller.isGrounded && _player != null && _player.State() == EntityState.Alive)
        {
            _player.ChangeState(EntityState.Air);
            _velocity.y = _jumpForce;
            _animator.SetTrigger("isJump");
            _animator.SetBool("isAir", true);
        }
    }

    #endregion

    public void SetCanMove(bool tri)
    {
        _canMove = tri;
    }
}