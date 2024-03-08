using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Project_Anxiety.Game.Managers;
using Project_Anxiety.Game.Utility;
using UnityEngine.InputSystem;

namespace Project_Anxiety.Game.Units
{
    public sealed class Player : CharacterBase
    {
        [ShowInInspector] public String PlayerName { get; private set; } = "Anon";
        public Health Health { get; private set; }
        public Stats Stats { get; private set; }
        public JobSystem JobSystem { get; private set; }
        public PlayerCombatSystem PlayerCombatSystem { get; private set; }
        public BuffSystem BuffSystem { get; private set; }
        public PlayerUI PlayerUI { get; private set; }
        public PlayerInput PlayerInput { get; private set; }


        private TouchingDirections _touchingDirections;
    
        private Vector2 moveInput;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpImpulse = 6f;
        [ShowInInspector, ReadOnly] private float crouchingSpeed => walkSpeed / 2;
    
        public float CurrentMoveSpeed
        {
            get
            {
                if (CanMove)
                {
                    if (IsMoving && !_touchingDirections.IsOnWall)
                    {
                        // Walking or Running
                        if (_touchingDirections.IsGrounded && !IsCrouching)
                        {
                            return IsRunning ? runSpeed + Stats.BonusMoveSpeed : walkSpeed + Stats.BonusMoveSpeed;
                        }
                        // Crouching
                        if (_isMoving && IsCrouching)
                        {
                            return crouchingSpeed;
                        }
                        // In Air
                        return IsRunning ? (runSpeed + Stats.BonusMoveSpeed) / 1.5f : (walkSpeed + Stats.BonusMoveSpeed) / 1.5f;
                    }

                    // Idle Speed
                    return 0;
                }

                // Movement is locked
                return 0;
            }
        }

        public float JumpImpulse => jumpImpulse;
    
        private bool _isMoving = false;
        public bool IsMoving
        {
            get { return _isMoving;}
            private set
            {
                _isMoving = value;
                Animator.SetBool(AnimationStrings.isMoving, value);
            }
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                Animator.SetBool(AnimationStrings.isRunning, value);
            }
        }

        private bool _isCrouching = false;
        public bool IsCrouching
        {
            get { return _isCrouching; }
            private set
            {
                _isCrouching = value;
                Animator.SetBool(AnimationStrings.isCrouching, value);
            }
        }

        public bool _isFacingRight = true;
        public bool IsFacingRight
        {
            get { return _isFacingRight;}
            private set
            {
                if (_isFacingRight != value)
                {
                    transform.localScale *= new Vector2(-1, 1);
                }

                _isFacingRight = value;
            }
        }

        public bool CanMove => Animator.GetBool(AnimationStrings.canMove);

        public bool KnockedBack = false;
    
        public override void Awake()
        {
            base.Awake();
            Health = GetComponent<Health>();
            Stats = GetComponent<Stats>();
            JobSystem = GetComponent<JobSystem>();
            PlayerCombatSystem = GetComponent<PlayerCombatSystem>();
            BuffSystem = GetComponent<BuffSystem>();
            PlayerUI = GetComponent<PlayerUI>();
            PlayerInput = GetComponent<PlayerInput>();
            _touchingDirections = GetComponent<TouchingDirections>();
        }

        private void FixedUpdate()
        {
            if (!KnockedBack)
            {
                RB.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, RB.velocity.y);
                Animator.SetFloat(AnimationStrings.yVelocity, RB.velocity.y);
            }
        }
    
        private void SetFacingDirection(Vector2 moveInput)
        {
            // TODO - Make sure the player doesnt turn when attacking.
            if (moveInput.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (moveInput.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }


        #region PlayerInput

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();

            if (!Health.IsDead)
            {
                IsMoving = moveInput.x is > 0 or < 0;
                SetFacingDirection(moveInput);
                IsCrouching = moveInput.y < 0;
            }
            else
            {
                IsMoving = false;
            }
        }
    
        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsRunning = true;
            }
            else if (context.canceled)
            {
                IsRunning = false;
            }
        
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            // TODO - Fix this whenever I want to be able to let player attack in the air.
            if (context.started && _touchingDirections.IsGrounded)
            {
                Animator.SetTrigger(AnimationStrings.attackTrigger);
                //PlayerCombatSystem.UseAbility(JobSystem.CurrentJob.JobInfo.JobAbilities.Find(x => x.Name == "BasicAttack"));
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            //if(context.started && _touchingDirections.IsGrounded && CanMove && )
            
            if (context.started && _touchingDirections.IsGrounded && CanMove)
            {
                Animator.SetTrigger(AnimationStrings.jumpTrigger);
            
                RB.velocity = new Vector2(RB.velocity.x, jumpImpulse + Stats.BonusJumpImpulse);
            }
        }

        public void OnOpenAbilityWindow(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PlayerUI.AbilityWindow.SetActive(!PlayerUI.AbilityWindow.activeSelf);
            }
        }

        public void OnOpenAttributeWindow(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PlayerUI.StatWindow.SetActive(!PlayerUI.StatWindow.activeSelf);
            }
        }

        public void OnHotBarAction(int slot)
        {
            PlayerCombatSystem.UseActiveAbility(HotBarManager.Instance.HotBarSlots[slot]?.activeAbilityData);
        }
        
        #endregion
    }
}
