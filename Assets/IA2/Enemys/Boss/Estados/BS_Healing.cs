using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;
using System;

public class BS_Healing : MonoBaseState
{
    Animator anims;
    Boss _owner;
  






    private void Awake()
    {
    
        _owner = GetComponent<Boss>();
    }



    public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
    {

    }

    public override void UpdateLoop()
    {
 

    }



    public override IState ProcessInput()
    {
        return this;
    }


    IEnumerator healTimer()
    {
        if (_owner.MaxLife < 500 && _owner.MaxLife > 0)
        {
            yield return new WaitForSeconds(2);
            _owner.Life += 10;
            StartCoroutine(healTimer());
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("HealingZone"))
        {
            StartCoroutine(healTimer());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("HealingZone"))
        {
            StopAllCoroutines();
        }
    }


}
