using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterSystems.Movement
{
    [RequireComponent(typeof(CharacterMovement3D))]
    public class ThirdPersonCamera3D : MonoBehaviour
    {
        [Header("Camera Movement Properties")]
        /// <summary>
        /// Controls the sensitivity on the horizontal (Y axis of rotation) axis
        /// </summary>
        public float CameraHorizontalSensitivity = 20f;
        /// <summary>
        /// Controls the sensitivity on the vertical (X axis of rotation) axis
        /// </summary>
        public float CameraVerticalSensitivity = 20f;
        /// <summary>
        /// Minimum X (Vertical) rotation
        /// </summary>
        public float MinXRotation = -5f;
        /// <summary>
        /// Maximum X (Vertical) rotation
        /// </summary>
        public float MaxXRotation = 90f;
        /// <summary>
        /// A curve that can be used to manipulate the distance of camera to player (VALUE) according to the X (Vertical) axis rotation (TIME). VALUE is the *multiplier* of distance
        /// </summary>
        public AnimationCurve XRotationToDistanceCurve = new AnimationCurve(new Keyframe(-5f, 0.5f), new Keyframe(35f, 0.9f, 0.005f, 0.005f), new Keyframe(90f, 1f));
        /// <summary>
        /// The normal distance of camera to the player
        /// </summary>
        public float CameraStandardDistance = 5f;
        [Header("Component References")]
        /// <summary>
        /// A reference to the camera transform. Doesn't *need* to be set as it will automatically select the main camera of the scene
        /// </summary>
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