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
    
    public CinemachineCamera vCamera;
    public float offsetAmount = 1f;
    public CinemachinePositionComposer positionComposer;
    public DialogueUI DialogueUI => dialogueUI;
    public Slider PneumaSlider;

    private bool hasHealing;
    
    public IInteractable Interactable { get; set; }
    
    private Rigidbody2D rb;
    private float baseOffsetX;

    private bool FacingLeft = false;

    public static event Action OnHealingGranted;

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
    }

    private void BasicAttack()
    {
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

    void FixedUpdate()
    {
        TurnCheck();
    }

    private void TurnCheck()
    {
        float moveDir = rb.linearVelocity.x; 

        if (moveDir > 0.01f && FacingLeft)
        {
            FacingLeft = false;
            Flip();
        }
        else if (moveDir < -0.01f && !FacingLeft)
        {
            FacingLeft = true;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += 180f;
        transform.eulerAngles = rotation;
        //TweenOnTurn();
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
        pneuma += 4;
        PneumaSlider.value = pneuma;
    }
}
