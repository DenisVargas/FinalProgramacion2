using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class ActiveWeapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Boss Boss = other.GetComponent<Boss>();
        if (Boss)
        {
            Boss.hasRangeWeapon = true;

            Destroy(gameObject);
        }
    }
}
