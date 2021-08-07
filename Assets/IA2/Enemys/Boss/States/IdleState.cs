using System;
using System.Collections.Generic;
using FSM2;
using UnityEngine;

public class IdleState : MonoBaseState 
{
    [SerializeField] Boss _owner;
    
    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        _owner.animator.SetBool("Walking", false);
    }

	public override Dictionary<string, object> Exit(IState to)
    {
        _owner.animator.SetBool("Idle", false);
        return base.Exit(to);
	}

	public override IState ProcessInput()
    {
        if (_owner.IsPlayerInSight() && Transitions.ContainsKey("OnChaseState"))
            return Transitions["OnChaseState"];

        return this;
    }

    public override void UpdateLoop()
    {
        throw new NotImplementedException();
    }
}
