using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;


public class BS_Chase : MonoBaseState
{
    Transform target;
    Boss _owner;
    Animator anim;
    [SerializeField] GameObject _doorLeft2;
    [SerializeField] GameObject _doorRight2;
    [SerializeField] GameObject _activeSpawner;



    private void Awake()
    {
        anim = GetComponent<Animator>();
        _owner = GetComponent<Boss>();
       

    }


    public override void Enter(IState from , Dictionary<string, object> transitionParameters = null)
    {
        anim.SetBool("Walking", true);
        _owner.LookTowardsPlayer = true;
        target = _owner.GetTarget();
    }


    public override void UpdateLoop()
    {


        if (target)
        {
            _owner.SetNavmeshDestination(target.position);


            _doorLeft2.SetActive(true);
            _doorRight2.SetActive(true);
            _activeSpawner.SetActive(true);



          
        }

        if (_owner.Life <= 0)
        {
            _owner.StopNavmeshNavigation();
        }

    }

    public override IState ProcessInput()
    {

      
        float distToTarget = Vector3.Distance(_owner.transform.position, target.position);

        if (distToTarget <= _owner.AttackRange && Transitions.ContainsKey("OnBS_Attack"))
        {
            return Transitions["OnBS_Attack"];
        }
        

        if (distToTarget <= _owner.DistantAttackRange && _owner.hasRangeWeapon == true && Transitions.ContainsKey("OnBS_RangeAttack"))
        {
            return Transitions["OnBS_RangeAttack"];
        }
        return this;


    }

    
}

