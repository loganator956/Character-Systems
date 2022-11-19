using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterSystems.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement3D : MonoBehaviour, IMovableCharacter
    {
        [Header("Movement Properties")]
        public float WalkSpeed = 3.0f;
        public float RunSpeed = 6.0f;
        public float MaxTurnSpeed = 720.0f;

        private Vector3 _targetVelocity;
        private Vector2 _inputs;
        private bool _isSprinting;

        public Vector3 CurrentDirection { get { return transform.forward; } }

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        #region User Input
        public void Move(Vector2 inputs)
        {
            _inputs= inputs.normalized;
        }

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
        #endregion


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
            _rb.velocity = new Vector3(_targetVelocity.x, _rb.velocity.y, _targetVelocity.z);
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
