using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;

public class BS_RangeAttack : MonoBaseState
{


     Transform _target;
     Boss _owner;
     Animator _anims;

    private void Awake()
    {
        _anims = GetComponent<Animator>();
        _owner = GetComponent<Boss>();
    }


    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        Debug.Log("===================== Trowing State ======================");
        _anims.SetBool("IsThrowing", true);
        _owner.isShooting = true;
        _target = _owner.GetTarget();
    }




    public override event Action OnNeedsReplan;
    public override void UpdateLoop()
    {
        Debug.Log("Update");
        float distanceToTarget = Vector3.Distance(_owner.transform.position, _target.position);

        if (distanceToTarget > _owner.DistantAttackRange)
        {
          
            OnNeedsReplan?.Invoke();
        }
            

    }

    public override IState ProcessInput()
    {


        return this;


    }

    public override Dictionary<string, object> Exit(IState to)
    {
        _owner.isAttacking = false;
        _anims.SetBool("IsThrowing", false);
        return base.Exit(to);


    }




}
