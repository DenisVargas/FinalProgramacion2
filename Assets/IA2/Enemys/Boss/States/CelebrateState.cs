using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;

public class CelebrateState : MonoBaseState
{
    [SerializeField] Boss _owner;

    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        _owner.animator.SetBool("Dance", true);
    }

    public override void UpdateLoop()
    {
    }

    public override IState ProcessInput()
    {
        return this;
    }
}
