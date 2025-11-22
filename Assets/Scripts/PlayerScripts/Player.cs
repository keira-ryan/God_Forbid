using System;
using System.Collections;
using DamageSystem;
using DG.Tweening;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private HitInfo hitInfo;
    [SerializeField] private Hurtbox hurtbox;
    [SerializeField] private int pneuma;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private FocusBlast blastPrefab;
    [SerializeField] private Transform bombSpawnTransform;
    [SerializeField] private Transform blastSpawnTransform;
    
    public CinemachineCamera vCamera;
    public float offsetAmount = 1f;
    public CinemachinePositionComposer positionComposer;
    public DialogueUI DialogueUI => dialogueUI;
    public Slider PneumaSlider;
    
    public event Action OnBasicAttack;

    private bool hasHealing;
    private bool hasBomb;
    private bool hasBlast;
    private bool isDead = false;
    
    public IInteractable Interactable { get; set; }
    
    private Rigidbody2D rb;
    private float baseOffsetX;

    //private bool FacingLeft = false;

    public static event Action OnHealingGranted;
    
    public static event Action OnDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        baseOffsetX = positionComposer.TargetOffset.x;
        hitbox.Deactivate();
        pneuma = 0;
        PneumaSlider.value = pneuma;
        hasHealing = false;
        hasBlast = false;
        hasBomb = false;
    }
    

    void Update()
    {
        if (dialogueUI.IsOpen) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable?.Interact(this);
        }

        if (Input.GetMouseButtonDown(0))
        {
            BasicAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Heal();
        }

        if (Input.GetKeyDown(KeyCode.Q) && hasBlast)
        {
            Blast();
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && hasBomb)
        {
            PlaceBomb();
        }

        if (hurtbox.CurrentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    private void PlaceBomb()
    {
        if (pneuma >= 3)
        {
            pneuma -= 3;
            PneumaSlider.value = pneuma;
            Instantiate(bombPrefab, bombSpawnTransform.position, Quaternion.identity);
        }
    }

    public void GiftPneuma()
    {
        pneuma += 10;
        PneumaSlider.value = pneuma;
    }

    private void Blast()
    {
        bool facingRight = transform.localScale.x > 0;
        if (pneuma >= 7) {
            pneuma -= 7;
            PneumaSlider.value = pneuma;
            
            FocusBlast blastProjectile = Instantiate(blastPrefab, blastSpawnTransform.position, Quaternion.identity);
            blastProjectile.Initialize(transform, facingRight);
        }
    }

    private void BasicAttack()
    {
        OnBasicAttack?.Invoke();
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        hitbox.Activate(hitInfo, direction);
        StartCoroutine(DeactivateHitboxAfterDelay(0.2f));
    }

    private IEnumerator DeactivateHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitbox.Deactivate();
    }

    private void Heal()
    {
        if (hasHealing && pneuma >= 5 && hurtbox.CurrentHealth < 20)
        {
            hurtbox.SetHealth(Mathf.Min(hurtbox.CurrentHealth + 5, 20));
            pneuma -= 5;
            PneumaSlider.value = pneuma;
        }
    }

    public void GrantHealing()
    {
        hasHealing = true;
        OnHealingGranted?.Invoke();
    }

    public void GrantBombs()
    {
        hasBomb = true;
    }

    public void GrantBlast()
    {
        hasBlast = true;
    }

    private void OnEnable()
    {
        if (hitbox != null)
            hitbox.OnHit.AddListener(incrementPneuma);
    }

    private void OnDisable()
    {
        hitbox.OnHit.RemoveListener(incrementPneuma);
    }

    private void incrementPneuma(HitEventInfo info)
    {
        if ((pneuma + 4) > 20)
        {
            pneuma = 20;
        }
        else
        {
            pneuma += 4;
        }
        PneumaSlider.value = pneuma;
    }
}
