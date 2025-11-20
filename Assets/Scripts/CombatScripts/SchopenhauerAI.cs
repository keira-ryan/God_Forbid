using System.Collections;
using DamageSystem;
using UnityEngine;

public class SchopenhauerAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    public float rageThreshold = 25f;
    public int maxHealth = 50;
    public bool isActive = false;
    public Hitbox basicHitbox;
    public Hitbox rageHitbox;
    

    private Rigidbody2D rb;
    private Hurtbox hurtbox;
    private enum BossState { Frozen, Move, BasicAttack, JumpAttack, RageAttack }
    private BossState currentState;
    private Animator animator;

    private void Start()
    {
        //identifying child components
        rb = GetComponent<Rigidbody2D>();
        hurtbox = GetComponentInChildren<Hurtbox>();
        animator = GetComponent<Animator>();
        
        //set up boss health 
        hurtbox.SetHealth(maxHealth);
        
        //
        currentState = BossState.Frozen;
        StartCoroutine(StateLoop());
    }

    private IEnumerator StateLoop()
    {
        while (true)
        {
            switch (currentState)
            {
                case BossState.Frozen:
                    break;
                case BossState.Move:
                    yield return StartCoroutine(Move());
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

    private void ChooseNextState()
    {
        if (!isActive)
        {
            currentState = BossState.Frozen;
            return;
        }
        if (hurtbox.CurrentHealth <= rageThreshold)
        {
            currentState = BossState.RageAttack;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > 5f)
        {
            currentState = BossState.JumpAttack;
        }
        else
        {
            currentState = (Random.value > 0.5f) ? BossState.BasicAttack : BossState.Move;
        }
    }

    private IEnumerator Move()
    {
        float moveTime = 1.5f;
        float timer = 0f;

        while (timer < moveTime)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private IEnumerator BasicAttack()
    {
        // Trigger basic attack animation here
        Debug.Log("Basic Attack!");
        yield return new WaitForSeconds(1f); // attack duration
    }

    private IEnumerator JumpAttack()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Optional: move horizontally while in air
        float jumpDuration = 0.5f;
        float timer = 0f;
        while (timer < jumpDuration)
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); 
    }

    private IEnumerator RageAttack()
    {
        // Trigger a more powerful attack pattern
        Debug.Log("Rage Attack!");
        yield return new WaitForSeconds(2f); // attack duration
    }
}
