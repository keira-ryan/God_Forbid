using System.Collections;
using DamageSystem;
using UnityEngine;

public class FocusBlast : MonoBehaviour
{
    [SerializeField] private float blastSpeed = 12f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask collisionMask;
    
    [SerializeField] private float blastDuration = 0.2f;
    [SerializeField] private HitInfo hitInfo;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private ParticleSystem particle;

    private bool hasExploded = false;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Vector2 direction;

    public void Initialize(Transform playerTransform, bool facingRight)
    {
        player = playerTransform;
        
        direction = facingRight ? Vector2.right : Vector2.left;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
        particle = GetComponentInChildren<ParticleSystem>();
        particle.gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (hasExploded) return;
        
        transform.Translate(direction * blastSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D detectedCollider)
    {
        Debug.Log("Hit " + detectedCollider.name + " on layer: " 
                  + LayerMask.LayerToName(detectedCollider.gameObject.layer));
        
        if (hasExploded) return;
        
        if (detectedCollider.transform.root == player) return;
        
        if ((collisionMask.value & (1 << detectedCollider.gameObject.layer)) == 0) return;

        StartCoroutine(Blast());
    }

    private IEnumerator Blast()
    {
        hasExploded = true;

        blastSpeed = 0f;
        
        spriteRenderer.enabled = false;
        
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        
        hitbox.Activate(hitInfo, direction);
        
        particle.gameObject.SetActive(true);
        particle.Play();
        
        yield return new WaitForSeconds(blastDuration);
        
        hitbox.Deactivate();
        Destroy(gameObject);
    }
    
}
