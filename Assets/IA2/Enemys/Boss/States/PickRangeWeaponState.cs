using System;
using FSM2;
using System.Collections.Generic;
using UnityEngine;

public class PickRangeWeaponState : MonoBaseState
{
    public override event Action OnNeedsReplan;
    [SerializeField] Boss _owner;

    public override void UpdateLoop()
    {
        _owner.GoToRangedWeapon();
    }

    public override IState ProcessInput()
    {
        if (!_owner.hasRangedWeapon)
            return this;

        if (_owner.IsPlayerInRange() && Transitions.ContainsKey("OnRangeAttackState"))
            return Transitions["OnRangeAttackState"];

        OnNeedsReplan?.Invoke();

        return this;
    }
}