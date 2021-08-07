using UnityEngine;

public class IddleHealerState<T> : State<T>
{
    public Animator Anims;
    public EnemyHealer _owner;

    public IddleHealerState(EnemyHealer enemyHealer, Animator enemyHardAnim)
    {
        Anims = enemyHardAnim;
        _owner = enemyHealer;
    }
}
