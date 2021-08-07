using System;
using FSM2;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackState : MonoBaseState {

    public override event Action OnNeedsReplan;

    [SerializeField] Boss _owner;

    float timer;

    float _timer;

   
    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        timer = 2f;
        _timer = 1f;
    }

    public override void UpdateLoop()
    {
        if (_owner.Life > 0)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            if (_timer > 0)
                _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _owner.isAttacking = true;
                _owner.StopNavmeshNavigation();
                _owner.animator.SetBool("BasicAttack", true);

               

                _owner.RangedAttack();
            }
        }
		
    }

    public override IState ProcessInput()
    {
        if (!_owner.IsPlayerAlive() && Transitions.ContainsKey("OnCelebrateState"))
            return Transitions["OnCelebrateState"];

        if (timer <= 0)
            OnNeedsReplan?.Invoke();

        return this;
    }
}