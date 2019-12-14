﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState<T> : State<T>
{
    EnemyBasic _owner;
    Animator anims;

    public DieState(EnemyBasic owner, Animator anim)
    {
        this._owner = owner;
        anims = anim;
    }

    public override void Enter()
    {
        anims.SetBool("IsDying", true);
        _owner.DisableEntity();
    }
}
