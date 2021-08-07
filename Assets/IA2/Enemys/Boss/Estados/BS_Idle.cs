using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;

public class BS_Idle : MonoBaseState
{
    Animator anims;
    Boss _owner;
    

    private Player _player;


    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _owner = GetComponent<Boss>();
    }

  

    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        // anims.SetBool("Walking", false);
        Debug.Log("entre al estado iddle");    
    }

    public override void UpdateLoop()
    {

        
    }



    public override IState ProcessInput()
    {
        if (_owner.IsInSight(_player.transform))
        {

            return Transitions["OnBS_Chase"];
        }
        return this;
    }
}
