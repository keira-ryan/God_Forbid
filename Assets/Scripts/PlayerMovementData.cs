using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement Data")]
public class PlayerMovementData : ScriptableObject
{
    [Header("Gravity Values")] 
    [HideInInspector] public float gravity;
    [HideInInspector] public float gravityScale;
    public float gravityFallMultiplier;
    public float gravityFallClamp;
    public float gravityDownFallMultiplier;
    public float gravityDownFallClamp;
    
    [Space(10)]
    [Header("Run Values")]
    public float runSpeed;
    public float runAcceleration;
    [HideInInspector] public float calcRunAcceleration;
    public float runDeceleration;
    [HideInInspector] public float calcRunDeceleration;
    [Range(0f,1)] public float airAcceleration;
    [Range(0f,1)] public float airDeceleration;
    public bool conserveMomentum = true;

    [Space(10)] 
    [Header("Jump Values")] 
    public float jumpValue;
    public float jumpPeakTime;
    [HideInInspector] public float jumpForce;
    public float quickJumpGravity;
    [Range(0f,1)] public float jumpHangGravity;
    public float jumpHangTime;
    public float jumpHangAcceleration;
    public float jumpHangMaxSpeed;
    
    [Space(10)] 
    [Header("Wall Jump Values")]
    public Vector2 wallJumpVelocity;
    [Range(0f, 1)] public float wallJumpLerp;
    [Range(0f, 1)] public float wallJumpTime;
    public bool turnOnJump = true;
    
    [Space(10)]
    [Header("Slide Values")]
    public float slideSpeed;
    public float slideAcceleration;

    [Space(10)] 
    [Header("Assist Values")] 
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpBufferTime;

    private void OnValidate()
    {
        gravity = -(2 * jumpValue) / (jumpPeakTime * jumpPeakTime);
        
        gravityScale = gravity / Physics2D.gravity.y;
        
        calcRunAcceleration = (50 * runAcceleration) / runSpeed;
        calcRunDeceleration = (50 * runDeceleration) / runSpeed;

        jumpForce = Mathf.Abs(gravity) * jumpPeakTime;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runSpeed);
        runDeceleration = Mathf.Clamp(runDeceleration, 0.01f, runSpeed);

    }

}
