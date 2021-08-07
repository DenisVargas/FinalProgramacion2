using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingState<T> : State<T>
{
    public Transform _target;
    public EnemyBasic _owner;
    public Animator _anims;

    public HealingState(EnemyBasic enemyBasic, Animator enemyHealerAnim)
    {
        _anims = enemyHealerAnim;
        _owner = enemyBasic;
    }


}
