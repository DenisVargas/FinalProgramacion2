using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IddleState<T> : State<T>
{
    public Animator Anims;
    public EnemyBasic _owner;

    public IddleState(EnemyBasic enemyBasic, Animator enemyBasicAnim)
    {
        Anims = enemyBasicAnim;
        _owner = enemyBasic;
    }

    public override void Enter()
    {
        //Debug.Log("Enemigo iddle");
        Anims.SetBool("IsWalking", false);
    }
}
