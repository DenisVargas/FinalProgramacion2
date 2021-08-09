using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM2;

public class Boss : Enemy
{
    public Action OnBossHasBeenKilled = delegate { };

    public float WaitTime = 3f;
    public bool isAttacking = false;
    public bool isShooting = false;

    [Header("Stun")]
    public bool stunned = false;
    public float StunTime = 4f;
    public float damageIncrease = 10;
    public float _distanceToWeapon;
    public float _distanceToHealZone;

    [Header("On Dead")]
    [SerializeField] Collider HurtBox = null;
    [SerializeField] Collider MainCollider = null;
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;
    [SerializeField] GameObject doorLeft2;
    [SerializeField] GameObject doorRight2;
    [SerializeField] GameObject ActiveSpawner;
    [SerializeField] GameObject RangeWeapon;
    [SerializeField] GameObject HealingZone;

    [Header("Combate")]
    public GameObject prefabProjectile = null;
    public Transform bulletSpawnPosition = null;


    public IdleState idleState;
    public ChaseState chaseState;
    public MeleeAttackState meleeAttackState;
    public RangeAttackState rangedAttackState;
    public PickRangeWeaponState pickRangeWeaponState;
    public CelebrateState celebrateState;

    private FiniteStateMachine _fsm;

    private float _lastReplanTime;
    private float _replanRate = .5f;

    public bool hasRangeWeapon = false;
    public Player Player { get => _player; }
    public Animator animator { get => _anims; }

    //Attacks
    [SerializeField] public float sightRange;
    [SerializeField] float meleeRange;
    [SerializeField] float rangedRange;
    [SerializeField] float rangedDamage;
    [SerializeField] float rangedThrowSpeed;
    [SerializeField] BossRangedWeapon rangedWeapon;
    [SerializeField] Transform rangedAttackPoint;
    public bool hasRangedWeapon = true;
    float timeSinceLastThrow;

    public bool LookTowardsHealingZone { get; set; } = false;
    public bool LookTowardsRangeWeapon { get; set; } = false;



    void Start()
    {
    
        idleState.OnNeedsReplan += OnReplan;
        chaseState.OnNeedsReplan += OnReplan;
        meleeAttackState.OnNeedsReplan += OnReplan;
        rangedAttackState.OnNeedsReplan += OnReplan;
        pickRangeWeaponState.OnNeedsReplan += OnReplan;
        celebrateState.OnNeedsReplan += OnReplan;

       
        PlanAndExecute();
    }

    protected override void Update()
    {
        

        timeSinceLastThrow += Time.deltaTime / 2;
    }

  

    private void PlanAndExecute()
    {
      
        var actions = new List<GOAPAction>{
                                              new GOAPAction("Idle")
                                                 .SetEffect("isPlayerInSight", true)
                                                 .SetLinkedState(idleState),

                                              new GOAPAction("Chase")
                                                 .SetPre("isPlayerInSight", true)
                                                 .SetEffect("isPlayerNear",    true)
                                                 .SetEffect("isPlayerInRange",    true)
                                                 .SetLinkedState(chaseState),

                                              new GOAPAction("Melee Attack")
                                                 .SetPre("isPlayerNear",   true)
                                                 .SetEffect("isPlayerAlive", false)
                                                 .SetLinkedState(meleeAttackState)
                                                 .SetCost(MeleeAttackCost() + 5),

                                              new GOAPAction("Ranged Attack")
                                                 .SetPre("isPlayerInRange", true)
                                                 .SetPre("hasRangeWeapon",  true)
                                                 .SetEffect("isPlayerAlive", false)
                                                 .SetLinkedState(rangedAttackState)
                                                 .SetCost(RangeAttackCost() + 1),

                                              new GOAPAction("Pick Ranged Weapon")
                                                 .SetPre("hasRangeWeapon", false)
                                                 .SetEffect("hasRangeWeapon", true)
                                                 .SetLinkedState(pickRangeWeaponState),

                                              new GOAPAction("Celebrate")
                                                 .SetPre("isPlayerAlive", false)
                                                 .SetEffect("Celebrate", true)
                                                 .SetLinkedState(celebrateState),
                                          };

        var from = new GOAPState();
        from.values["isPlayerInSight"] = IsPlayerInSight();
        from.values["isPlayerNear"] = IsPlayerNear();
        from.values["isPlayerAlive"] = IsPlayerAlive();
        from.values["hasRangeWeapon"] = hasRangedWeapon;
        from.values["isPlayerInRange"] = IsPlayerInRange();

        var to = new GOAPState();
        to.values["Celebrate"] = true;

        var planner = new GoapPlanner(StartCoroutine);

        planner.Run(from, to, actions);
        planner.OnPathCreated += ConfigureFsm;

      
    }

    private void OnReplan()
    {
        if (Time.time >= _lastReplanTime + _replanRate)
        {
            _lastReplanTime = Time.time;
        }
        else
        {
            return;
        }

        PlanAndExecute();

        
    }

    private void ConfigureFsm(IEnumerable<GOAPAction> plan)
    {
        Debug.Log("Completed Plan");
        _fsm = GoapPlanner.ConfigureFSM(plan, StartCoroutine, _fsm);
        _fsm.Active = true;
    }


    public override void DisableEntity()
    {
        OnBossHasBeenKilled();
        
            
        HurtBox.enabled = false;
        MainCollider.enabled = false;
        enabled = false;
    }

    public void DistanceToWeapon()
    {
        _distanceToWeapon = Vector3.Distance(RangeWeapon.transform.position, this.transform.position); 
    }

    public void DistanceToHealZone()
    {
        _distanceToHealZone = Vector3.Distance(HealingZone.transform.position, this.transform.position);
    }

    //=================================== MEMBER FUNCS ===========================================

    public Transform GetTarget()
    {
        if (_target)
            return _target;
        else return null;
    }

    public Transform GetTargetWeaponPos()
    {
        if (_WeaponPos)
            return _WeaponPos;
        else return null;
    }

    public Transform GetTargetHealingZonePos()
    {
        if (_HealZonePos)
            return _HealZonePos;
        else return null;
    }

    public void Shoot()
    {
        //Instancio un projectil.
        Bullet newbullet = Instantiate(prefabProjectile, bulletSpawnPosition.position, Quaternion.identity)
                          .GetComponent<Bullet>();
        Vector3 dirToTarget = (_target.transform.position - bulletSpawnPosition.position).normalized;
        newbullet.transform.forward = dirToTarget;

        newbullet.SetOwner(this);
        newbullet.Damage = _Rangedamage;
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
                Destroy(doorLeft);
                Destroy(doorRight);
                ActiveSpawner.SetActive(false);
                result.targetEliminated = true;
                GameProgressTracker.BossEnemyKilled();
                StopNavmeshNavigation();
                _anims.SetBool("Dead", true);
                _anims.SetBool("BasicAttack", false);
                DisableEntity();
            }
        }

        return result;
        //Este boss es imparable asi que aunque reciba daño seguira su patrón de forma fiel.
    }

    //=================================== IA2 ========================================
    public override HitData GetCombatStats()  
    {
        return new HitData()
        {
            Damage = _damage
        };
    }
    public bool IsPlayerNear()
    {
        return (_target.transform.position - transform.position).magnitude < meleeRange;
    }
    public bool IsPlayerInRange()
    {
        return (_target.transform.position - transform.position).magnitude < rangedRange;
    }

    public bool IsPlayerInSight()
    {
        var dirToPlayer = _player.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        return (Physics.Raycast(transform.position, dirToPlayer, sightRange) && angleToPlayer <= 180);
    }

    public bool IsPlayerAlive()
    {
        return _player._health >= 0;
    }

    float MeleeAttackCost()
    {
        return (_target.transform.position - transform.position).magnitude;
    }
    float RangeAttackCost()
    {
        return (rangedWeapon.transform.position - transform.position).magnitude / Math.Min(1, timeSinceLastThrow);
    }
    public void GoToRangedWeapon()
    {
        SetNavmeshDestination(rangedWeapon.transform.position);

        if ((rangedWeapon.transform.position - transform.position).magnitude < 2f)
        {
            rangedWeapon.PickWeapon(rangedAttackPoint);
            hasRangedWeapon = true;

           
        }
    }


    public void RangedAttack()
    {
        if (!hasRangedWeapon)
            return;
        
        rangedWeapon.transform.position = rangedAttackPoint.position;
        rangedWeapon.transform.LookAt(_player.transform.position, Vector3.up);

        rangedWeapon.ThrowWeapon(rangedThrowSpeed, rangedDamage);

        hasRangedWeapon = false;

        timeSinceLastThrow = 0f;
    }
}
