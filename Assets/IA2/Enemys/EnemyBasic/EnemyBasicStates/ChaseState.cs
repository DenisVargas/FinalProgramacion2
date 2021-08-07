using UnityEngine;
using UnityEngine.AI;

public class ChaseState<T> : State<T>
{
    Transform target;
    EnemyBasic _owner;
    Animator anim;

    public ChaseState(EnemyBasic enemyBasic, Transform target, Animator anim)
    {
        _owner = enemyBasic;
        this.anim = anim;
        this.target = target;
    }

    public override void Enter()
    {
        anim.SetBool("IsWalking", true);
        _owner.LookTowardsPlayer = true;
    }

    public override void Update()
    {
        _owner.SetNavmeshDestination(target.position);
        float distToTarget = Vector3.Distance(_owner.transform.position, target.position);

        if (distToTarget <= _owner.AttackRange)
            _owner.Sm.Feed(EnemyBasic.BE_Inputs.IsNear);
    }
}
