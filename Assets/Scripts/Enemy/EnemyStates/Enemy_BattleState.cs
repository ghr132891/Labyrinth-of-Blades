using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeInBattle;
    private Transform lastTarget;
    
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();

        if (player == null)
            player = enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            // 【修改点】：不要直接用 rb.linearVelocity 赋值，统一使用 enemy.SetVelocity！
            // 这样撤退也能应用底层反转和翻转贴图逻辑
            enemy.SetVelocity((enemy.reteratVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.reteratVelocity.y);
            //rb.linearVelocity = new Vector2((enemy.reteratVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.reteratVelocity.y);

            enemy.HandleFlip(DirectionToPlayer());
        }

        

    }

    public override void Update()
    {
        base.Update();


        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver())
            stateMachine.ChangeState(enemy.idleState);
       

        if (WithinAttackRange() && enemy.PlayerDetected())
            stateMachine.ChangeState(enemy.attackState);
        else
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);

       

    }

    private void UpdateTargetIfNeeded()
    {
        if(enemy.PlayerDetected() == false)
            return;

        Transform newTarget = enemy.PlayerDetected().transform;

        if(newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }



    }
    
    private void UpdateBattleTimer() => lastTimeInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeInBattle + enemy.battleTimeDuration;

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minAbleRetreatDistance;
    
    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        float verticalDistance = Mathf.Abs(player.position.y - enemy.transform.position.y);
        float horizonalDistance = Mathf.Abs(player.position.x - enemy.transform.position.x);
        if (verticalDistance > .1f && horizonalDistance < 0.1f)
            return 0;

        int dir = player.position.x > enemy.transform.position.x ? 1 : -1;

        // 【删除】底下这段代码！
        /* if (WorldManager.Instance != null && WorldManager.Instance.currentWorld == WorldType.Mirror)
        {
            dir = -dir;
        }
        */

        return dir;
    }

}
