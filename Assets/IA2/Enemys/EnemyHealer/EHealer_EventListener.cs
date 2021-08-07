using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHealer_EventListener : MonoBehaviour
{
    EnemyHealer _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<EnemyHealer>();
    }

    //El combate siempre tiene 3 estados básicos: StartUp, Active, Recover
    void StartUp()
    {
        _owner.LookTowardsPlayer = true;
    }

    void Active()
    {
        _owner.Shoot();
        _owner.LookTowardsPlayer = false;
    }

    //void Recover()
    //{

    //}

    //void EndAttackAnimation()
    //{

    //}

    //Cuando termina el hit hacemos algo.
    void EndHitAnimation()
    {
       
        _owner.SM.Feed(EnemyHealer.EH_Inputs.NoTakingDamage);
    }
}
