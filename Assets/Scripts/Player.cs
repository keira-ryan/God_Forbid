using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    
    public CinemachineCamera vCamera;
    public float offsetAmount = 1f;
    public CinemachinePositionComposer positionComposer;
    public DialogueUI DialogueUI => dialogueUI;
    
    public IInteractable Interactable { get; set; }
    
    private Rigidbody2D rb;
    private float baseOffsetX;

    private bool FacingLeft = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        baseOffsetX = positionComposer.TargetOffset.x;
    }
    

    void Update()
    {
        if (dialogueUI.IsOpen) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable?.Interact(this);
        }
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

    /*
    private void TweenOnTurn()
    {
        float targetOffsetX = FacingLeft 
            ? baseOffsetX - Mathf.Abs(offsetAmount) 
            : baseOffsetX + Mathf.Abs(offsetAmount);

        float currentOffsetX = positionComposer.TargetOffset.x;

        DOTween.To(
            () => currentOffsetX,
            x =>
            {
                currentOffsetX = x;
                positionComposer.TargetOffset.x = currentOffsetX;
            },
            targetOffsetX,
            0.25f
        ).SetEase(Ease.OutSine);
    }
    */
}
