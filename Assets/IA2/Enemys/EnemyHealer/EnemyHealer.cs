using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyHealer : Enemy
{
    public enum EH_Inputs
    {
        IsInSigth,
        IsNotInSigth,
        TakingDamage,
        NoTakingDamage,
        WeakAllyNearby,
        IsDead
    }
    public FSM<EH_Inputs> SM;



    public bool isHealing;

    public SpatialGrid targetGrid;



    [SerializeField] Collider HurtBox = null;
    [SerializeField] Collider MainCollider = null;


    [Header("Curacion")]
    [SerializeField] float healRange = 5f;
    [SerializeField] float healing = 10f;
    [SerializeField] int healTargets = 1;
    [SerializeField] float healCooldown = 5f;
    public ParticleSystem healParticle; 
    float healTimer = 0;

    //=================================== UNITY FUNCS ============================================

    protected override void Awake()
    {
        base.Awake(); //El Awake base rellena los componentes básicos.

        _agent.enabled = false;
        LookTowardsPlayer = false;

        //State Machine.
        var Iddle = new IddleHealerState<EH_Inputs>(this, _anims);
      
        var GetHit = new TakeHealerDamageState<EH_Inputs>(this, _anims);
        var Heal = new HealingHealerState<EH_Inputs>(this, _anims);
        var Die = new DieHealerState<EH_Inputs>(this, _anims);

        Iddle.AddTransition(EH_Inputs.TakingDamage, GetHit)
             .AddTransition(EH_Inputs.IsDead, Die)
             .AddTransition(EH_Inputs.WeakAllyNearby, Heal);

    

        GetHit.AddTransition(EH_Inputs.NoTakingDamage, Iddle)
              .AddTransition(EH_Inputs.IsDead, Die)
              .AddTransition(EH_Inputs.WeakAllyNearby, Heal);

        Heal.AddTransition(EH_Inputs.IsDead, Die)
            .AddTransition(EH_Inputs.IsNotInSigth, Iddle)
            .AddTransition(EH_Inputs.TakingDamage, GetHit);

        SM = new FSM<EH_Inputs>(Iddle);
    }

    protected override void Update()
    {
        if (_life <= 0)
        {
            SM.Feed(EH_Inputs.IsDead);
            return;
        }

        base.Update();
        SM.Update();

        if (CanHeal() && NeedToHeal())
            SM.Feed(EH_Inputs.WeakAllyNearby);

        else if (IsInSight(_target))
            SM.Feed(EH_Inputs.IsInSigth);

        if (CanHeal())
            Debug.Log("CAN HEAL");
        if (NeedToHeal())
            Debug.Log("NEED TO HEAL");

        if (healTimer > 0)
            healTimer -= Time.deltaTime;

    }

    public override void DisableEntity()
    {
        HurtBox.enabled = false;
        MainCollider.enabled = false;

        enabled = false;
    }

    //=================================== FUNCIONES MIEMBRO ======================================

    public void Shoot()
    {
       
    }
    public Transform GetCurrentTarget()
    {
        if (_target)
            return _target.transform;
        else
            return null;
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
                GameProgressTracker.NotifyRangeEnemyKilled();
                SM.Feed(EH_Inputs.IsDead);
            }
            else if (CanGetCriticalHit)
                SM.Feed(EH_Inputs.TakingDamage);
        }

        return result;
    }




    public void Heal()
    {
        var alliesToHeal = GetAlliesToHeal();
        foreach (var ally in alliesToHeal)
        {
            ally.Life = Mathf.Min(ally.MaxLife, ally.Life + healing);
        }

        healTimer = healCooldown;
    }

   // IA2-P2 y IA2-P3
    IEnumerable<Enemy> GetAlliesToHeal()
    {
        return targetGrid.Query(transform.position + new Vector3(-healRange, 0, -healRange),
                        transform.position + new Vector3(healRange, 0, healRange),
                        x => Vector3.Distance(x, transform.position) <= healRange)
            .Select(entity => entity.GameObject.GetComponent<Enemy>())
            .Where(enemy => enemy.Life <= enemy.MaxLife / 2 && enemy.Life > 0)
            .OrderBy(enemy => enemy.Life)
            .Take(healTargets);
    }


    bool NeedToHeal()
    {
        return GetAlliesToHeal().Count() > 0;
    }

    bool CanHeal()
	{
        return healTimer <= 0;
	}
}