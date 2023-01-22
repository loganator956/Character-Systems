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
        public float MinXRotation = -5f;
        public float MaxXRotation = 90f;
        public AnimationCurve XRotationToDistanceCurve = new AnimationCurve(new Keyframe(-5f, 0.5f), new Keyframe(35f, 0.9f, 0.005f, 0.005f), new Keyframe(90f, 1f));
        public float CameraStandardDistance = 5f;
        [Header("Component References")]
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (CameraTransform == null)
            {
                Debug.LogWarning("Camera transform is not set, auto-detecting");
                CameraTransform = Camera.main.transform;
        }}

        private void Update()
        {
            // add inputs
            _yAngle += _inputs.x * CameraHorizontalSensitivity * Time.deltaTime;
            _xAngle += _inputs.y * CameraVerticalSensitivity * Time.deltaTime;

            // clamp
            if (_yAngle > 360) { _yAngle -= 360; } else if (_yAngle < 0) { _yAngle += 360; };
            _xAngle = Mathf.Clamp(_xAngle, MinXRotation, MaxXRotation);

            // apply
            CameraTransform.localRotation = Quaternion.Euler(_xAngle, _yAngle, 0f);
            CameraTransform.position = transform.position + CameraTransform.forward * -1 * CameraStandardDistance * XRotationToDistanceCurve.Evaluate(_xAngle);
            if (_characterMovement3D != null)
            {
                Vector3 fwd = CameraTransform.forward;
                fwd.y = 0f;
                _characterMovement3D.CameraForward = fwd;
            }
        }
    }
}