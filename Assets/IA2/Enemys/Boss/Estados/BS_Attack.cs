using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;


public class BS_Attack : MonoBaseState
{
    Boss _owner;
    Animator Anims;
    Transform _target;

    private void Awake()
    {
        Anims = GetComponent<Animator>();
        _owner = GetComponent<Boss>();
    }


    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
   
        Anims.SetBool("BasicAttack", true);
        _owner.isAttacking = true;
        _owner.StopNavmeshNavigation();
        _target = _owner.GetTarget();
    }


    public override Dictionary<string, object> Exit(IState to)
    {
       
        _owner.isAttacking = false;
        return base.Exit(to);
    }

    public override event Action OnNeedsReplan;

    public override void UpdateLoop()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        if (distanceToTarget > _owner.AttackRange && _owner.IsInSight(_target)) //Condición de salida del ataque.
        {
            Anims.SetBool("BasicAttack", false);
            OnNeedsReplan?.Invoke();


        }

    }



    public override IState ProcessInput()
    {
        return this;
    }
}


