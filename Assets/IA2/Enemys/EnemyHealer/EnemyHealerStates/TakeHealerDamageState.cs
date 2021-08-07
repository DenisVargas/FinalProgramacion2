using UnityEngine;

public class TakeHealerDamageState<T> : State<T>
{
    public Animator anims;
    public EnemyHealer _owner;

    public TakeHealerDamageState(EnemyHealer enemyHealer, Animator enemyHealerAnim)
    {
        anims = enemyHealerAnim;
        _owner = enemyHealer;
    }

    public override void Enter()
    {
        Debug.Log("Enemigo recibe daño");
        anims.SetBool("ReceiveDamage", true);
        _owner.CanGetCriticalHit = false;
    }
    public override void Exit()
    {
        anims.SetBool("ReceiveDamage", false);
    }
}
