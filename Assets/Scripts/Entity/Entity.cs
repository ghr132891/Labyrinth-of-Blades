using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{

    public event Action OnFlipped;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    [Header("collision detection")]
    public LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    //Condition variables.
    private bool isKnocked;
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;


    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();

        

    }


   

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public virtual void StopSlowDown()
    {
        slowDownCo = null;
    }
    public virtual void SlowDownEntity(float duration,float slowMultiplier,bool canOverrideSlowEffect = false)
    {
        if(slowDownCo != null)
        {
            if (canOverrideSlowEffect)
                StopCoroutine(slowDownCo);
            else
                return;
        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration,slowMultiplier));

    }

    protected virtual IEnumerator SlowDownEntityCo(float duration ,float slowMultiplier)
    {
        yield return null;
    }


    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {

    }

    public void ReceiveKnockback(Vector2 knockback,float duration)
    {
        if(knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback,duration));

    }

    public IEnumerator KnockbackCo(Vector2 knockback,float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        float finalXVelocity = xVelocity;

        if(WorldManager.Instance != null && WorldManager.Instance.currentWorld == WorldType.Mirror)
        {
            if(this.GetComponent<Player>() == null)
                finalXVelocity = -xVelocity; 
        }

        rb.linearVelocity = new Vector2(finalXVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
            Flip();
        else if (xVelocity < 0 && facingRight)
            Flip();

        OnFlipped?.Invoke();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        int actualFacingDir = facingDir;

        if(WorldManager.Instance != null && WorldManager.Instance.currentWorld == WorldType.Mirror)
        {
            if (this.GetComponent<Player>() == null)
            {
                actualFacingDir = -facingDir;
            }
        }

        if (secondaryWallCheck != null)
        {
        wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * actualFacingDir, wallCheckDistance, whatIsGround)
                    && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * actualFacingDir, wallCheckDistance, whatIsGround);
        }
        else
            wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * actualFacingDir, wallCheckDistance, whatIsGround);

    }

    protected virtual void OnDrawGizmos() 
    {
        int actualFacingDir = facingDir;
        if (WorldManager.Instance != null && WorldManager.Instance.currentWorld == WorldType.Mirror)
        {
            actualFacingDir = -facingDir;
        }

        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * actualFacingDir, 0));

        if (secondaryWallCheck != null)
        {
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * actualFacingDir, 0));
        }
    }

    
}
