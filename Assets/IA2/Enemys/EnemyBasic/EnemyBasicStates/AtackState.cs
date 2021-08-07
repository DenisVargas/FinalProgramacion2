using UnityEngine;

public class AtackState<T> : State<T>
{
    EnemyBasic _owner;
    Animator Animator;

    public AtackState( EnemyBasic owner, Animator enemyBasicAnim)
    {
        this.Animator = enemyBasicAnim;
        _owner = owner;
    }

    public override void Enter()
    {
        Animator.SetBool("IsAtacking", true);
        _owner.isAttacking = true;
        _owner.StopNavmeshNavigation();
    }
    public override void Exit()
    {
        _owner.isAttacking = false;
    }
}
