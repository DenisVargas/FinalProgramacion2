using System.Collections.Generic;
using FSM2;
using UnityEngine;

public class ChaseState : MonoBaseState {

    [SerializeField] Boss _owner;
    [SerializeField] GameObject _doorLeft2;
    [SerializeField] GameObject _doorRight2;

    private void Awake()
    {
        _owner = GetComponent<Boss>();
    }
    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {
        _owner.animator.SetBool("Walking", true);
        _doorLeft2.SetActive(true);
        _doorRight2.SetActive(true);
        _owner.sightRange = 30;
    }

    public override void UpdateLoop()
    {
        _owner.SetNavmeshDestination(_owner.Player.transform.position);
        if (_owner.Life <= 0)
        {
            _owner.StopNavmeshNavigation();
        }
    }

    public override IState ProcessInput()
    {
        if (_owner.IsPlayerInRange() && _owner.hasRangedWeapon && Transitions.ContainsKey("OnRangeAttackState"))
        {
            return Transitions["OnRangeAttackState"];
        }

        if (_owner.IsPlayerNear() && Transitions.ContainsKey("OnMeleeAttackState")) {
            return Transitions["OnMeleeAttackState"];
        }

        return this;
    }
}