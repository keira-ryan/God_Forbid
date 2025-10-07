using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementData Data;

    public Rigidbody2D Player { get; private set; }

    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    public float LastGroundedTime { get; private set; }
    public float LastWallTime { get; private set; }
    public float LastWallRightTime { get; private set; }
    public float LastWallLeftTime { get; private set; }

    public float LastPressedJumpTime { get; private set; }

    private bool isJumpShort;
    private bool isJumpFalling;

    private float wallStartTime;
    private int lastWallDirection;

    private Vector2 movementInput;

    [Header("Surface Checks")] 
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] Transform frontWallCheckPoint;
    [SerializeField] Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        Player = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        UpdateTimers();

        //input handling
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.x != 0)
            CheckDirectionToFace(movementInput.x > 0);

        //jump checks
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W))
        {
            OnJumpInput();
        }

        //collision checks
        if (!IsJumping)
        {
            //ground check
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !IsJumping)
            {
                LastGroundedTime = Data.coyoteTime;
            }

            //right wall check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && IsFacingRight)
                 || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) &&
                     !IsFacingRight)) && !IsWallJumping)
            {
                LastWallRightTime = Data.coyoteTime;
            }

            //left wall check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !IsFacingRight)
                 || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) &&
                     IsFacingRight)) && !IsWallJumping)
            {
                LastWallLeftTime = Data.coyoteTime;
            }

            LastWallTime = Mathf.Max(LastWallLeftTime, LastWallRightTime);
        }

        //jump checks
        if (IsJumping && Player.linearVelocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - wallStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastGroundedTime > 0 && !IsJumping && !IsWallJumping)
        {
            isJumpShort = false;

            if (!IsJumping)
                isJumpShort = false;
        }

        //trigger jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            isJumpShort = false;
            isJumpFalling = false;
            Jump();
        }
        //trigger wall jump
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            isJumpShort = false;
            isJumpFalling = false;
            wallStartTime = Time.time;
            lastWallDirection = (LastWallRightTime > 0) ? -1 : 1;
            WallJump(lastWallDirection);
        }

        //slide checks
        if (CanSlide() && ((LastWallLeftTime > 0 && movementInput.x < 0) ||
                           (LastWallRightTime > 0 && movementInput.x > 0)))
        {
            IsSliding = true;
        }
        else
        {
            IsSliding = false;
        }

        //gravity modifications
        if (IsSliding)
        {
            SetGravityScale(0);
        }
        else if (Player.linearVelocity.y < 0 && movementInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.gravityDownFallMultiplier);
            //caps max fall speed
            Player.linearVelocity = new Vector2(Player.linearVelocity.x,
                Mathf.Max(Player.linearVelocity.y, -Data.gravityDownFallClamp));
        }
        else if (isJumpShort)
        {
            //fall faster if pressing down
            SetGravityScale(Data.gravityScale * Data.gravityFallMultiplier);
            Player.linearVelocity = new Vector2(Player.linearVelocity.x,
                Mathf.Max(Player.linearVelocity.y, -Data.gravityDownFallClamp));
        }
        else if ((IsJumping || IsWallJumping || isJumpFalling) &&
                 Mathf.Abs(Player.linearVelocity.y) < Data.jumpHangTime)
        {
            SetGravityScale(Data.gravityScale * Data.gravityFallMultiplier);
            Player.linearVelocity = new Vector2(Player.linearVelocity.x,
                Mathf.Max(Player.linearVelocity.y, -Data.gravityDownFallClamp));
        }
        else
        {
            SetGravityScale(Data.gravityScale);
        }
    }

    private void FixedUpdate()
    {
        if (IsWallJumping)
            Run(Data.wallJumpLerp);
        else
        {
            Run(1);
        }

        if (IsSliding)
            Slide();
    }

    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpShort() || CanWallJumpShort())
            isJumpShort = true;
    }

    public void SetGravityScale(float scale)
    {
        Player.gravityScale = scale;
    }
    
    private void UpdateTimers()
    {
        LastGroundedTime -= Time.deltaTime;
        LastWallTime -= Time.deltaTime;
        LastWallRightTime -= Time.deltaTime;
        LastWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
    }

    private void Run(float lerpAmount)
    {
        float targetSpeed = movementInput.x * Data.runSpeed;
        targetSpeed = Mathf.Lerp(Player.linearVelocity.x, targetSpeed, lerpAmount);

        float accelerationRate;
        if (LastGroundedTime > 0)
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.calcRunAcceleration : Data.calcRunDeceleration;
        else
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.calcRunAcceleration * Data.airAcceleration : Data.calcRunDeceleration * Data.airDeceleration;
        
        //add bonus acceleration at jump peak
        if ((IsJumping || IsWallJumping || isJumpFalling) && Mathf.Abs(Player.linearVelocity.y) < Data.jumpHangTime)
        {
            accelerationRate *= Data.jumpHangAcceleration;
            targetSpeed *= Data.jumpHangMaxSpeed;
        }
        
        //conserve momentum
        if (Data.conserveMomentum && Mathf.Abs(Player.linearVelocity.x) > Mathf.Abs(targetSpeed) &&
            Mathf.Sign(Player.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f &&
            LastGroundedTime < 0)
        {
            accelerationRate = 0;
        }
        
        float speedDifference = targetSpeed - Player.linearVelocity.x;
        float movement = speedDifference * accelerationRate;

        float maxSpeed = Data.runSpeed;

        if (Mathf.Abs(Player.linearVelocity.x) < maxSpeed)
        {
            Player.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

    }

    private void Turn()
    {
        //flip player along x-axis for wall jump
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        //update facing direction
        IsFacingRight = !IsFacingRight;
    }

    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastGroundedTime = 0;
        
        float force = Data.jumpForce;
        if (Player.linearVelocity.y > 0)
            force -= Player.linearVelocity.y;
        Player.AddForce(force * Vector2.up, ForceMode2D.Impulse);
    }

    private void WallJump(int direction)
    {
        LastPressedJumpTime = 0;
        LastGroundedTime = 0;
        LastWallRightTime = 0;
        LastWallLeftTime = 0;
        
        Vector2 force = new Vector2(Data.wallJumpVelocity.x, Data.wallJumpVelocity.y);
        force.x *= direction;
        
        if (Mathf.Sign(Player.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= Player.linearVelocity.x;
        if (Player.linearVelocity.y < 0)
            force.y -= Player.linearVelocity.y;
        
        Player.AddForce(force, ForceMode2D.Impulse);
    }

    private void Slide()
    {
        float speedDifference = Data.slideSpeed - Player.linearVelocity.y;
        float movement = speedDifference * Data.slideAcceleration;
        
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDifference) * (1/Time.fixedDeltaTime), Mathf.Abs(speedDifference) * (1/Time.fixedDeltaTime));

        Player.AddForce(movement * Vector2.up);
    }

    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastGroundedTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastWallTime > 0 && LastGroundedTime <= 0 && (!IsWallJumping || 
                (LastWallRightTime > 0 && lastWallDirection == 1) || (LastWallLeftTime > 0 && lastWallDirection == -1));
    }

    private bool CanJumpShort()
    {
        return IsJumping && Player.linearVelocity.y > 0;
    }

    private bool CanWallJumpShort()
    {
        return IsWallJumping && Player.linearVelocity.y > 0;
    }

    private bool CanSlide()
    {
        if (LastWallTime > 0 && !IsJumping && !IsWallJumping && LastGroundedTime <= 0)
            return true;
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
    }
}
