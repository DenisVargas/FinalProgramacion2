using System;
using FSM2;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : MonoBaseState
{
    public override event Action OnNeedsReplan;
    [SerializeField] float attackCooldown = .5f;
    [SerializeField] Boss _owner;

    float _lastAttackTime;
    float timer;

    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        timer = 2f;
    }

    public override void UpdateLoop() {

        timer -= Time.deltaTime;
        if (Time.time >= _lastAttackTime + attackCooldown) {
            _lastAttackTime = Time.time;
            _owner.animator.SetBool("BasicAttack", true);
            _owner.isAttacking = true;
            _owner.StopNavmeshNavigation();
        }
        
    }

    public override IState ProcessInput()
    {
        if (!_owner.IsPlayerAlive() && Transitions.ContainsKey("OnCelebrateState"))
            return Transitions["OnCelebrateState"];

        if (_owner.IsPlayerNear())
            return this;

        if(timer <= 0)
            OnNeedsReplan?.Invoke();

        return this;
    }
}