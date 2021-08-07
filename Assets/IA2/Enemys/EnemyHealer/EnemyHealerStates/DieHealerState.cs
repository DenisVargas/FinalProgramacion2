using UnityEngine;

public class DieHealerState<T> : State<T>
{
    EnemyHealer _owner;
    Animator anims;

    public DieHealerState(EnemyHealer owner, Animator anim)
    {
        _owner = owner;
        anims = anim;
    }

    public override void Enter()
    {
        anims.SetBool("IsDying", true);
        _owner.DisableEntity();
    }
}
