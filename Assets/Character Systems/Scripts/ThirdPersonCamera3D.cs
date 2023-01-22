using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterSystems.Movement
{
    [RequireComponent(typeof(CharacterMovement3D))]
    public class ThirdPersonCamera3D : MonoBehaviour
    {
        [Header("Camera Movement Properties")]
        public float CameraHorizontalSensitivity = 20f;
        public float CameraVerticalSensitivity = 20f;
        public float MinXRotation = -90f;
        public float MaxXRotation = 90f;
        public Transform CameraTransform;

        private Vector2 _inputs;
        private float _xAngle, _yAngle;
        private CharacterMovement3D _characterMovement3D;

        public void OnLook(InputValue value)
        {
            _inputs = value.Get<Vector2>();
        }

        private void Awake()
        {
            _characterMovement3D = GetComponent<CharacterMovement3D>();
        }

        private void Update()
        {
            // add inputs
            _yAngle += _inputs.x * CameraHorizontalSensitivity * Time.deltaTime;
            _xAngle += _inputs.y * CameraVerticalSensitivity * Time.deltaTime;

            // clamp
            _yAngle = Mathf.Clamp(_yAngle, 0f, 360f);
            _xAngle = Mathf.Clamp(_xAngle, MinXRotation, MaxXRotation);

            // apply
            CameraTransform.localRotation = Quaternion.Euler(_xAngle, _yAngle, 0f);
            if (_characterMovement3D != null)
            {
                Vector3 fwd = CameraTransform.forward;
                fwd.y = 0f;
                _characterMovement3D.CameraForward = fwd;
            }
        }
    }
}