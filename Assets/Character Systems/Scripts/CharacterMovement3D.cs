using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterSystems.Movement
{
    /// <summary>
    /// This movement behaviour is designed more for a top-down 3D game where camera angle doesn't change much
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement3D : MonoBehaviour, IMovableCharacter
    {
        [Header("Movement Properties")]
        public float WalkSpeed = 3.0f;
        public float RunSpeed = 6.0f;
        public float AccelerationRate = 6.0f;
        public float MaxTurnSpeed = 360.0f;

        private Vector2 _inputs;
        private bool _isSprinting;

        public Vector3 CurrentDirection { get { return transform.forward; } }

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Implements interface that can be called by other scripts
        /// </summary>
        /// <param name="inputs"></param>
        public void Move(Vector2 inputs)
        {
            _inputs= inputs.normalized;
        }

        /// <summary>
        /// Implements OnMove message from PlayerInput component
        /// </summary>
        /// <param name="value"></param>
        public void OnMove(InputValue value)
        {
            Move(value.Get<Vector2>());
        }

        public void SetSprint(bool isSprint)
        {
            _isSprinting = isSprint;
        }

        public void OnSprint(InputValue value)
        {
            SetSprint(value.Get<float>() == 1f);
        }

        private Vector3 _targetVelocity;

        private void Update()
        {
            _targetVelocity = InputToWorldDirection(_inputs);
            if (_isSprinting)
                _targetVelocity *= RunSpeed;
            else
                _targetVelocity *= WalkSpeed;
            RotateTowardDirection();
        }

        private void RotateTowardDirection()
        {
            if (_inputs == Vector2.zero) return;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(InputToWorldDirection(_inputs), Vector3.up), MaxTurnSpeed * Time.deltaTime);
        }


        private void FixedUpdate()
        {
            float intendedAcceleration = Mathf.Abs(_targetVelocity.magnitude - _rb.velocity.magnitude);
            float maxAccelPerFixedUpdate = AccelerationRate * Time.fixedDeltaTime;
            float t = Mathf.Min(maxAccelPerFixedUpdate, intendedAcceleration);
            
            // This calculates how far between (lerp) current velocity and target velocity to set new velocity to
            // I needed to ensure that do not divide zero by intendedAcceleration, however, so it checks to make sure :)
            _rb.velocity = Vector3.Lerp(_rb.velocity, _targetVelocity, (t != 0 ? t / intendedAcceleration : t));
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + InputToWorldDirection(_inputs), 0.1f);
        }

        private Vector3 InputToWorldDirection(Vector2 input)
        {
            return new Vector3(input.x, 0, input.y).normalized;
        }
    }
}
