using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BS_Stunned<T> : State<T>
{
    Animator anims;
    Boss _owner;
    float stunTime;

    float time = 0;

    public BS_Stunned(Boss owner, Animator anims, float stunTime)
    {
        _owner = owner;
        this.anims = anims;
        this.stunTime = stunTime;
    }

    public override void Enter()
    {
        anims.SetBool("Stunned", true);
        _owner.stunned = true;
        _owner.StopNavmeshNavigation();
    }
    public override void Update()
    {
        if (time <= 0) return;

        time -= Time.deltaTime;
        if (time <= 0)
            anims.SetBool("Stunned", false);
    }
    public override void Exit()
    {
        _owner.stunned = false;
    }
}
