using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToHealState<T> : State<T>
{
    Transform target;
    EnemyBasic _owner;
    Animator anim;

    public GoToHealState(EnemyBasic enemyBasic, Transform target, Animator anim)
    {
        _owner = enemyBasic;
        this.anim = anim;
        this.target = target;
    }

    public override void Enter()
    {
        base.Enter();   
    }

    public override void Update()
    {
        base.Update();
    }




}
