using System;
using UnityEngine;

namespace Framework.Components
{
    [Serializable]
    public class JumpProperties
    {
        public float BackSpeedMultiplier = 0.5f;
        public float StrafeSpeedMultiplier = 0.25f;
        public float VerticalSpeed = 0.3f;
    }
    class FreeformMovementComponent : MonoBehaviour
    {
        public bool IsGravityEnabled = true;
        public bool ShouldMoveRelativeToCamera;
        public float FallingMoveSpeedMultiplier = 1f;
        public float Gravity = -2.0f;
        public float GroundFriction = 15.0f;
        public float MoveSpeed = 3.0f;
        public JumpProperties JumpProperties;

        public bool IsJumping { get; private set; }
        public Vector3 JumpDirection { get; private set; }
        public Vector3 MoveDirection { get; private set; }

        private bool _wasGroundedLastFrame;
        private Vector3 _velocity;

        /// <summary>
        /// Call from FixedUpdate.
        /// </summary>
        public void Move(CharacterController characterController, Vector3 inputDirection)
        {
            if (!characterController)
            {
                DebugEx.LogError<FreeformMovementComponent>("Character Controller cannot be null!");
                return;
            }

            MoveDirection = GetMovementDirectionFromInputDirection(inputDirection);

            if (MoveDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(MoveDirection);

            var movementSpeedModifier = GetMovementSpeedModifier(characterController, MoveDirection);

            _velocity += MoveDirection.Multiply(movementSpeedModifier.Abs()) * Time.deltaTime;
            _velocity.x = GetFriction(_velocity.x, GroundFriction);
            _velocity.z = GetFriction(_velocity.z, GroundFriction);

            if (characterController.isGrounded)
            {
                // Hack fix to keep CharacterController grounded.
                if (_velocity.y < 0f)
                    _velocity.y = Gravity * Time.deltaTime;

                if (IsJumping && !_wasGroundedLastFrame)
                {
                    IsJumping = false;
                    JumpDirection = Vector3.zero;
                }
            }
            else if (!characterController.isGrounded)
            {
                if (IsGravityEnabled)
                    _velocity.y = _velocity.y + (Gravity * Time.deltaTime);
            }

            _wasGroundedLastFrame = characterController.isGrounded;

            characterController.Move(_velocity);
        }

        public void ApplyForce(Vector3 direction, float force, bool isRelative)
        {
            IsJumping = false;
            _velocity += (isRelative ? GetMovementDirectionFromInputDirection(direction) : direction) * force;
        }

        public void Jump(CharacterController characterController)
        {
            if (characterController.isGrounded && !IsJumping)
            {
                IsJumping = true;
                JumpDirection = _velocity.normalized.Multiply(Vector3Ex.XZ);
                _velocity += Vector3.up * JumpProperties.VerticalSpeed;
            }
        }

        /// <summary>
        /// Call this when the Jump button has been released for a shorter jump.
        /// </summary>
        public void ShortJump()
        {
            if (IsJumping)
                _velocity.y = _velocity.y * 0.5f;
        }

        private float GetFriction(float velocity, float friction)
        {
             return Mathf.Lerp(velocity, 0f, friction * Time.deltaTime);
        }

        private Vector3 GetMovementDirectionFromInputDirection(Vector3 inputDirection)
        {
            if (ShouldMoveRelativeToCamera)
            {
                if (!Camera.main)
                    return Vector3.zero;

                return Quaternion.Euler(new Vector3(0f, Camera.main.transform.eulerAngles.y, 0f)) * inputDirection;
            }

            return inputDirection;
        }

        private Vector3 GetMovementSpeedModifier(CharacterController characterController, Vector3 moveDirection)
        {
            if (characterController.isGrounded)
            {
                return Vector3.one * MoveSpeed;
            }
            else
            {
                if (IsJumping && JumpDirection != Vector3.zero)
                {
                    var forwardSpeed = Vector3.Dot(JumpDirection, moveDirection) < 0.0f ? MoveSpeed * JumpProperties.BackSpeedMultiplier : MoveSpeed;
                    var strafeSpeed = MoveSpeed * JumpProperties.StrafeSpeedMultiplier;
                    return Quaternion.LookRotation(JumpDirection) * new Vector3(strafeSpeed, 0f, forwardSpeed);
                }
                else
                {
                    return Vector3.one * FallingMoveSpeedMultiplier;
                }
            }
        }
    }
}
