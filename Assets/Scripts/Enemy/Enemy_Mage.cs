using UnityEngine;

public class Enemy_Mage : Enemy,ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_MageRetreatState mageRetreatState { get; private set; }
    public Enemy_MageBattleState mageBattleState { get; private set; }

    [Header("Mage Specifics")]
    [SerializeField] private bool hasStunRecoveryAnimation = true;

    [Space]
    public float retreatCoolDown =5;
    public float retreatMaxDistance = 8;
    public float retreatSpeed = 15;
    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        mageRetreatState = new Enemy_MageRetreatState(this, stateMachine, "battle");
        mageBattleState = new Enemy_MageBattleState(this, stateMachine, "battle");
        battleState = mageBattleState;

        anim.SetBool("hasStunRecovery", hasStunRecoveryAnimation);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void HandleCounter()
    {
        if (CanBeCountered == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
