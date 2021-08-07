using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;

public class BS_PickRangeWeapon : MonoBaseState
{

    Transform _target;
    [SerializeField] GameObject _rangeWeapon;
    Boss _owner;
    Animator anim;
 



    private void Awake()
    {
        anim = GetComponent<Animator>();
        _owner = GetComponent<Boss>();
       


    }


    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        anim.SetBool("Walking", true);
        _owner.LookTowardsRangeWeapon = true;
        _target = _owner.GetTargetWeaponPos();
    }


    public override void UpdateLoop()
    {


        if (_target)
        {
            _owner.SetNavmeshDestination(_rangeWeapon.transform.position);


        }

    
    }

    public override IState ProcessInput()
    {


        float distToTarget = Vector3.Distance(_owner.transform.position, _rangeWeapon.transform.position);
        if (_owner.transform.position == _rangeWeapon.transform.position)
        {
            _owner.hasRangeWeapon = true;
            return Transitions["OnBS_Chase"];

        }

       return this;


    }


}
