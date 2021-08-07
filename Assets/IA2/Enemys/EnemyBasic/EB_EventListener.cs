using UnityEngine;

[AddComponentMenu("EventListeners/Enemies/Basic", 1)]
public class EB_EventListener : MonoBehaviour
{
    EnemyBasic _owner = null;
    [SerializeField] Collider Hitbox = null;
    private void Awake()
    {
        _owner = GetComponentInParent<EnemyBasic>();
    }

    /// <summary>
    /// Se llama cuando la animacion de un hit recibido se termina.
    /// </summary>
    void HurtEnd()
    {
        _owner.Sm.Feed(EnemyBasic.BE_Inputs.NoTakingDamage);
        _owner.StartCoroutine(_owner.CriticalHitCoolDown());
    }

    //El combate en general tiene 3 fases: Start Up, Active y Recovery

    /// <summary>
    /// Podriamos usar esto para avisar que se inicio la animaciíon de combate.
    /// </summary>
    void StartUp()
    {
        _owner.LookTowardsPlayer = true;
    }
    /// <summary>
    /// Activamos el Hitbox.
    /// </summary>
    void Active()
    {
        if (Hitbox) Hitbox.enabled = true;
        _owner.LookTowardsPlayer = false;
    }
    /// <summary>
    /// Se desactiva el HitBox
    /// </summary>
    void Recovery()
    {
        if (Hitbox) Hitbox.enabled = false;
        _owner.EvaluateTarget();
    }
    /// <summary>
    /// Aviso al Owner que dejé de golpear.
    /// </summary>
    void EndAttackAnimation()
    {
        _owner.ExecuteEvaluateAction();
    }
}
