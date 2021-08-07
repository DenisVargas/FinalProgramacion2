using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    IFighter<HitData, HitResult> _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<IFighter<HitData, HitResult>>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != _owner.gameObject)
        {
            var target = other.GetComponentInParent<IFighter<HitData, HitResult>>();

            if (target != null)
                target.Hit(_owner.GetCombatStats());
        }
    }
}
