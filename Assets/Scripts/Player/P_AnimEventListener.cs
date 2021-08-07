using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_AnimEventListener : MonoBehaviour
{
    Player _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<Player>();
    }

    void EndReload()
    {
        _owner.CurrentWeapon.OnEndReloading();
    }

    void StartShoot()
    {
        print("START SHOOT EVENT");
        _owner.CurrentWeapon.isFiring = true;
    }
    void EventShoot()
    {
        _owner.CurrentWeapon.Shoot();
    }
    void EndShoot()
    {
        _owner.CurrentWeapon.isFiring = false;
        _owner.CurrentWeapon.lockFire = false;
    }

    void HurtStarted()
    {
        //El jugador recibe daño y no se puede mover.
    }
    void HurtEnded()
    {
        //El jugador terminó la animación y ya se puede mover de vuelta.
    }
    /// <summary>
    /// Evento que se llama cuando la animación de muerte se ha terminado.
    /// </summary>
    void Died()
    {
        Game.LoadScene(sceneIndex.Defeat);
    }
}
