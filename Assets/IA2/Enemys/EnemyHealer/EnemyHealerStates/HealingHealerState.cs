using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHealerState<T> : State<T>
{
    public Transform _target;
    public EnemyHealer _owner;
    public Animator _anims;

    public HealingHealerState(EnemyHealer enemyHealer, Animator enemyHealerAnim)
    {
        _anims = enemyHealerAnim;
        _owner = enemyHealer;
    }

    public override void Enter()
    {
        _anims.SetBool("IsHealing", true);
        _owner.isHealing = true;
        _target = _owner.GetCurrentTarget();
        _owner.Heal();
        _owner.healParticle.Play();
    }
    public override void Update()
    {
        float distanceToTarget = Vector3.Distance(_owner.transform.position, _target.position);

        if (distanceToTarget > _owner.AttackRange)
            _owner.SM.Feed(EnemyHealer.EH_Inputs.IsNotInSigth);
    }

    public override void Exit()
    {
        _owner.isHealing = false;
        _anims.SetBool("IsHealing", false);
    }
}
