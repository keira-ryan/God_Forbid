using UnityEngine;

namespace ControllerSystem.Platformer2D
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlatformerAnimator : EntityAnimator
    {
        // These should match the names of the Aseprite animations
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int BasicAttack = Animator.StringToHash("Attack");
        
        [SerializeField] private float _jumpAnimationLength = 0.25f;
        [SerializeField] private FighterController _fighterController;
        [SerializeField] private PlatformerJumpModule _platformerJumpModule;
        [SerializeField] private PlatformerCrouchModule _crouchModule;
        [SerializeField] private PlatformerWallModule _platformerWallModule;
        [SerializeField] private Player _player;
        private PlatformerMotor PlatformerMotor => (PlatformerMotor)motor;
        private float _lockInJumpAnimationUntilTime;
        private float _lockInAttackAnimationUntilTime;

        protected override void Awake()
        {
            base.Awake();
            _platformerJumpModule.OnJump += PlatformerMovement_OnJump;
            _player.OnBasicAttack += AnimateBasicAttack;
        }

        private void AnimateBasicAttack()
        {
            float attackDuration = 0.25f;
            _lockInAttackAnimationUntilTime = Time.time + attackDuration;
            SwitchAnimState(BasicAttack);
        }
        
        
        protected virtual void OnDestroy()
        {
            if (_platformerJumpModule != null)
            {
                _platformerJumpModule.OnJump -= PlatformerMovement_OnJump;
            }

            if (_player != null)
            {
                _player.OnBasicAttack -= AnimateBasicAttack;
            }
        }

        protected virtual void PlatformerMovement_OnJump(PositionInfo e)
        {
            _lockInJumpAnimationUntilTime = Time.time + _jumpAnimationLength;
        }

        protected override void Animate()
        {
            base.Animate();
            HandleAnimation();
        }

        private void HandleAnimation()
        {
            // If attack or jump animation is currently locked in, don't change
            if (Time.time < _lockInAttackAnimationUntilTime || Time.time < _lockInJumpAnimationUntilTime)
                return;

            int state = _fighterController.CurrentState == FighterController.States.Dead ? Dead : GetMovementState();
            SwitchAnimState(state);
        }

        private int GetMovementState()
        {
            int state;
            if (PlatformerMotor.Grounded)
            {
                state = GetGroundedState();
            }
            else
            {
                state = GetAirState();
            }

            return state;
        }

        /*
        protected virtual int GetWallState()
        {
            return _platformerWallModule.State switch
            {
                PlatformerWallModule.WallState.Cling => WallCling,
                PlatformerWallModule.WallState.Slide => WallSlide,
                PlatformerWallModule.WallState.Climb => WallClimb,
                PlatformerWallModule.WallState.Hang => WallHang,
                _ => -1
            };
        }
        */

        protected virtual int GetGroundedState()
        {
            int state;
            if (motor.Controller.InputtingHorizontalMovement && Mathf.Abs(motor.Rb.linearVelocity.x) > 0.1f)
            {
                state = Run;
            }
            else
            {
                state = Idle;
            }
            return state;
        }
        
        protected virtual int GetAirState()
        {
            int state;
            if (_platformerJumpModule.Rising || _lockInJumpAnimationUntilTime >= Time.time)
            {
                state = Jump;
            }
            else
            {
                state = Fall;
            }

            return state;
        }
    }
    
}