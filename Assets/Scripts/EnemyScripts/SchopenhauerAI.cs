using System;
using System.Collections;
using DamageSystem;
using UnityEngine;

public class SchopenhauerAI : MonoBehaviour
{
    [SerializeField] private float basicAttackRange = 2.0f;
    [SerializeField] private float rageAttackRange = 5.0f;
    [SerializeField] private HitInfo rageHitInfo;
    [SerializeField] private HitInfo basicHitInfo;
    [SerializeField] private ParticleSystem rageParticles;
    public Transform player;
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    public float sideForce = 8f;
    public float rageThreshold = 25f;
    public int maxHealth = 50;
    public bool isActive = false;
    public Hitbox basicHitbox;
    public Hitbox rageHitbox;
    
    private bool isJumping = false;
    private Rigidbody2D rb;
    private Hurtbox hurtbox;
    private enum BossState { Frozen, BasicAttack, JumpAttack, RageAttack }
    private BossState currentState;
    private Animator animator;

    private bool isPhaseTwo = false;
    private bool isDead = false;
    
    public static event Action OnDeath;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hurtbox = GetComponentInChildren<Hurtbox>();
        animator = GetComponentInChildren<Animator>();
        
        hurtbox.SetHealth(maxHealth);
        
        currentState = BossState.Frozen;
    }

    private void Update()
    {
        if (hurtbox.CurrentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
            
        }
    }

    private IEnumerator StateLoop()
    {
        while (true)
        {
            FacePlayer();
            switch (currentState)
            {
                case BossState.Frozen:
                    break;
                case BossState.BasicAttack:
                    yield return StartCoroutine(BasicAttack());
                    break;
                case BossState.JumpAttack:
                    yield return StartCoroutine(JumpAttack());
                    break;
                case BossState.RageAttack:
                    yield return StartCoroutine(RageAttack());
                    break;
            }
            ChooseNextState();
            yield return null;
        }
    }

    private void FacePlayer()
    {
        if (player == null) return;
        
        float direction = player.position.x - transform.position.x;

        if (direction < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void ChooseNextState()
    {
        float distanceToPlayer = Vector2.Distance(player.position, transform.position);
        if (hurtbox.CurrentHealth < rageThreshold)
        {
            isPhaseTwo = true;
        }
        
        if (!isActive)
        {
            currentState = BossState.Frozen;
            return;
        }

        if (!isPhaseTwo)
        {
            if (distanceToPlayer < basicAttackRange)
            {
                currentState = BossState.BasicAttack;
            }
            else if (!isJumping)
            {
                currentState = BossState.JumpAttack;
            }
            return;
        }

        if (distanceToPlayer <= rageAttackRange)
        {
            currentState = BossState.RageAttack;
            return;
        }

        currentState = BossState.JumpAttack;
    }
    public void StartEncounter()
    {
        Debug.Log("Encounter");
        isActive = true;
        currentState = BossState.JumpAttack;
        StartCoroutine(StateLoop());
    }

    private IEnumerator BasicAttack()
    {
        Debug.Log("Basic ecnounter");
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        basicHitbox.Activate();
        yield return new WaitForSeconds(0.2f);
        basicHitbox.Deactivate();
        
        animator.CrossFade("Idle", 0f);
        
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator JumpAttack()
    {
        Debug.Log("Jump ecnounter");
        
        isJumping = true;
        
        float xDir = (player.position.x < transform.position.x) ? -1f : 1f;
        
        rb.linearVelocity = Vector2.zero;
        
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        
        rb.AddForce(new Vector2(xDir * sideForce, 0), ForceMode2D.Impulse);

        animator.CrossFade("Idle", 0f);
        yield return new WaitForSeconds(2f);
        
        isJumping = false;
    }

    private IEnumerator RageAttack()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        rageParticles.gameObject.SetActive(true);
        rageParticles.Play();
        
        rageHitbox.Activate(rageHitInfo, Vector2.zero);
        
        yield return new WaitForSeconds(0.2f);
        
        rageHitbox.Deactivate();
        
        animator.CrossFade("Idle", 0f);
        
        yield return new WaitForSeconds(0.5f);
        
    }
}
