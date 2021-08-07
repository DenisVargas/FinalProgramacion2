using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRangedWeapon : MonoBehaviour
{
    public float _damage;
    private float timeToTurnOnCollider;

    [SerializeField] Transform initialTransform;
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider collider;

  
	private void Update()
	{
        if (timeToTurnOnCollider > 0)
            timeToTurnOnCollider -= Time.deltaTime;

        if (timeToTurnOnCollider <= 0)
            collider.enabled = true;
    }

	public void PickWeapon(Transform anchorpoint)
	{
        transform.position = anchorpoint.position;
        collider.enabled = false;
        transform.SetParent(anchorpoint);
        rb.useGravity = false;
    }

    public void ThrowWeapon(float throwSpeed, float throwerDamage)
	{
        //DEBUG
        Debug.Log("Weapon thrown.");


        transform.parent = null;
        rb.AddForce(transform.forward * throwSpeed);
        _damage = throwerDamage;
        timeToTurnOnCollider = 0.2f;
        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player)
                player.Health -= Mathf.RoundToInt(_damage);
        }
    }
}
