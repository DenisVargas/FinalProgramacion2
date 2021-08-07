using UnityEngine;

public class TakeDamageState<T> : State<T>
{
    public Animator _anims;
    public EnemyBasic _owner;

    public TakeDamageState(EnemyBasic enemyBasic, Animator enemyBasicAnim)
    {
        _anims = enemyBasicAnim;
        _owner = enemyBasic;
    }

    public override void Enter()
    {
        _anims.SetBool("GetHit", true);
        _owner.CanGetCriticalHit = false;
        _owner.StopNavmeshNavigation();
    }
    public override void Exit()
    {
        _anims.SetBool("GetHit", false);
    }
}
