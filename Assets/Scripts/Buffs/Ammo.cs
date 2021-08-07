using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ammo : MonoBehaviour
{
    [SerializeField] WeaponType bulletType = WeaponType.AssaultRifle;
    [SerializeField] int Ammount = 20;
    [SerializeField] float _lifeTime = 15f;
    [SerializeField] bool permanent = true;

    private void Update()
    {
        if (permanent) return;

        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            player.AddBullets(bulletType, Ammount);
            Destroy(gameObject);
        }
    }
}

