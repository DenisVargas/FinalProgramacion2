using System.Collections;
using UnityEngine;

public class EnemyBasic : Enemy
{
    public bool isAttacking = false;

    [SerializeField] Collider HurtBox = null;
    [SerializeField] Collider HitBox = null;
    [SerializeField] Collider MainCollider = null;

    public enum BE_Inputs
    {
        IsInSigth,
        IsNotInSigth,
        IsNear,
        IsNotNear,
        TakingDamage,
        NoTakingDamage,
        IsDead,
        NeedsToHeal,
        Healing

    }
    public FSM<BE_Inputs> Sm;
    BE_Inputs evaluation = BE_Inputs.IsNear;

    protected override void Awake()
    {
        base.Awake(); //El Awake base rellena los componentes básicos.

        //State Machine.
        var Iddle = new IddleState<BE_Inputs>(this, _anims);
        var chase = new ChaseState<BE_Inputs>(this, _target, _anims);
        var atack = new AtackState<BE_Inputs>(this, _anims);
        var goToHeal = new GoToHealState<BE_Inputs>(this, _target, _anims);
        var IsHealing = new HealingState<BE_Inputs>(this, _anims);
        var GetHit = new TakeDamageState<BE_Inputs>(this, _anims);
        var Die = new DieState<BE_Inputs>(this, _anims);

        Iddle.AddTransition(BE_Inputs.IsInSigth, chase)
             .AddTransition(BE_Inputs.IsDead, Die)
             .AddTransition(BE_Inputs.TakingDamage, GetHit)
             .AddTransition(BE_Inputs.NeedsToHeal, goToHeal);


        chase.AddTransition(BE_Inputs.IsDead, Die)
             .AddTransition(BE_Inputs.IsNotInSigth, Iddle)
             .AddTransition(BE_Inputs.IsNear, atack)
             .AddTransition(BE_Inputs.TakingDamage, GetHit)
             .AddTransition(BE_Inputs.NeedsToHeal, goToHeal);



        atack.AddTransition(BE_Inputs.IsNotNear, chase)
             .AddTransition(BE_Inputs.IsDead, Die)
             .AddTransition(BE_Inputs.TakingDamage, GetHit)
             .AddTransition(BE_Inputs.NeedsToHeal, goToHeal);

        GetHit.AddTransition(BE_Inputs.NoTakingDamage, Iddle)
              .AddTransition(BE_Inputs.IsDead, Die);

        Sm = new FSM<BE_Inputs>(Iddle);
    }

    protected override void Update()
    {
        if (_life <= 0)
            Sm.Feed(BE_Inputs.IsDead);

        base.Update();
        Sm.Update();

        if (IsInSight(_target))
            Sm.Feed(BE_Inputs.IsInSigth);
    }

    public override void DisableEntity()
    {
        HitBox.enabled = false;
        HurtBox.enabled = false;
        MainCollider.enabled = false;
        _agent.enabled = false;
        //Destroy(gameObject, 5f);

        this.enabled = false;
    }

    public override void EvaluateTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);
        if ( distanceToTarget > AttackRange && IsInSight(_target)) //Condición de salida del ataque.
        {
            _anims.SetBool("IsAtacking", false);
            evaluation = BE_Inputs.IsNotNear;
        }
    }
    public override void ExecuteEvaluateAction()
    {
        if (evaluation == BE_Inputs.IsNotNear)
            Sm.Feed(evaluation);
    }

    //=================================== Debugg Gizmos ==========================================

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        var position = transform.position;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, _range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, _angle / 2, 0) * transform.forward * _range);
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, -_angle / 2, 0) * transform.forward * _range);

        if (_target)
            Gizmos.DrawLine(position, _target.position);
    }

    //=================================== Sistema de Daño ========================================

    public override HitResult Hit(HitData hitData)
    {
        HitResult result = new HitResult();

        if (hitData.Damage > 0)
        {
            _life -= hitData.Damage;
            result.Conected = true;

            if (_life <= 0)
            {
                result.targetEliminated = true;
                GameProgressTracker.NofitySimpleEnemyKilled();
                Sm.Feed(BE_Inputs.IsDead);
            }
            else if(CanGetCriticalHit)
                Sm.Feed(BE_Inputs.TakingDamage);
        }

        return result;
    }

    public override HitData GetCombatStats()
    {
        return new HitData()
        {
            Damage = _damage
        };
    }

    public IEnumerator CriticalHitCoolDown()
    {
        CanGetCriticalHit = false;
        yield return new WaitForSeconds(CriticalhitCooldownTime);
        CanGetCriticalHit = true;
    }
}
