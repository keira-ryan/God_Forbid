using System.Collections;
using DamageSystem;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private HitInfo hitInfo;
    public float explosionTimer = 2.0f;
    public float flashDuration = 1.0f;
    public float explosionDuration = 0.2f;

    public int explosionDamage = 10;
    public LayerMask damageLayer;

    private SpriteRenderer bombSprite;
    private Hitbox hitbox;
    private ParticleSystem particle;
    
    void Awake()
    {
        bombSprite =  GetComponent<SpriteRenderer>();
        hitbox = GetComponentInChildren<Hitbox>();
        particle = GetComponentInChildren<ParticleSystem>();

        if (particle != null)
            particle.gameObject.SetActive(false);

    }

    void Start()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(explosionTimer);
        
        yield return StartCoroutine(FlashWarning());
        
        StartCoroutine(Explode());

        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }

    private IEnumerator FlashWarning()
    {
        float timePassed = 0f;
        bool bombVisible = true;

        while (timePassed < flashDuration)
        {
            bombSprite.enabled = bombVisible;
            bombVisible = !bombVisible;
            yield return new WaitForSeconds(0.1f);
            timePassed += 0.1f;
        }

        bombSprite.enabled = true;
    }

    private IEnumerator Explode()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        hitbox.Activate(hitInfo, direction);

        particle.gameObject.SetActive(true);
        particle.Play();
        
        yield return new WaitForSeconds(explosionDuration);
        hitbox.Deactivate();
    }
    
}
