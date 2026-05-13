using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private Transform _cinemachineFollowTarget;
    [SerializeField] private float _cameraAngleOverride = 0f;

    [Header("View Control")]
    [SerializeField] private float _lookSensitivity = 0.5f;
    [SerializeField] private float _topClamp = 70f;
    [SerializeField] private float _bottomClamp = -30f;
    [SerializeField] private bool _lockCameraPosition = false;

    private float _targetYaw;
    private float _targetPitch;
    private Vector2 _lookInput;

    private const float Threshold = 0.01f;

    private void Start()
    {
        _targetYaw = _cinemachineFollowTarget.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (_lookInput.sqrMagnitude >= Threshold && _lockCameraPosition == false)
        {
            _targetYaw += _lookInput.x * _lookSensitivity;
            _targetPitch -= _lookInput.y * _lookSensitivity;
        }

        _targetYaw = ClampAngle(_targetYaw, float.MinValue, float.MaxValue);
        _targetPitch = ClampAngle(_targetPitch, _bottomClamp, _topClamp);

        // Cinemachine이 이 타겟의 회전을 추적
        _cinemachineFollowTarget.rotation = Quaternion.Euler(
            _targetPitch + _cameraAngleOverride, _targetYaw, 0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    #region PlayerInput Send Messages

    private void OnView(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    #endregion
}
