using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerHUD))]
public class Player : MonoBehaviour, IFighter<HitData,HitResult>
{
    [Header("Inspector")]
    [SerializeField] FixedJoystick _joystick = null;
    [SerializeField] Transform _worldForward = null;

    [Header("Sonido")]
    [SerializeField] AudioSource AS_characterSounds;
    [SerializeField] AudioSource AS_GunSounds;
    [SerializeField] AudioClip S_PistolShoot;
    [SerializeField] AudioClip S_PistolReload;
    [SerializeField] AudioClip S_RifleShoot;
    [SerializeField] AudioClip S_RifleReload;
    [SerializeField] AudioClip S_WithoutAmmo;
    [SerializeField] AudioClip S_GetHurt;

    [Header("Enviroment Events")]
    [SerializeField] LayerMask _targeteables = 0;
    [SerializeField] float _targetDetectionRange = 10f;
    [SerializeField] IFighter<HitData, HitResult> _target = null;
    public GameObject FindedTarget = null;

    [Header("Stats")]
    [SerializeField] public int _health = 100;
    [SerializeField] int _maxHealth = 100;

    [SerializeField] RuntimeAnimatorController _pistolAnimations = null;
    [SerializeField] RuntimeAnimatorController _rifleAnimations = null;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            _hud.SetHealthDisplay(value);
        }
    }
    [SerializeField] float _movementSpeed = 5f;

    [Header("Equipment")]
    public Weapon CurrentWeapon = null;
    [SerializeField] Dictionary<WeaponType, Weapon> Weapons = new Dictionary<WeaponType, Weapon>();

    [Header("Snap to Ground")]
    [SerializeField] LayerMask _groundLayer = 0;
    [SerializeField] float _upOffset = 1f;

    PlayerHUD _hud = null;
    Rigidbody _rb = null;
    Animator _anim = null;

    Vector3 _axisDirection = Vector3.zero;
    Vector3 _movementDirection = Vector3.zero;
    Vector3 _raycastOrigin = Vector3.zero;
    Vector3 hitPoint = Vector3.zero;

    public bool lockInput;

    public bool IsHurted
    {
        get;
        set;
    }
    public bool IsAlive => _health > 0;

    public bool ShootPhase { get; private set; }

    private void Awake()
    {
        _hud = GetComponent<PlayerHUD>();
        Health = _maxHealth;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();

        var AviableWeapons = GetComponentsInChildren<Weapon>();
        Weapons = new Dictionary<WeaponType, Weapon>();
        foreach (var weaponComp in AviableWeapons)
        {
            weaponComp.SetOwner(this);
            weaponComp.StartShootAnimation += () => _anim.SetBool("IsFiring", true);
            weaponComp.StopShootAnimation += () => _anim.SetBool("IsFiring", false);
            weaponComp.StartReloadAnimation += () => _anim.SetBool("IsReloading", true);
            weaponComp.EndReloadAnimation += () => _anim.SetBool("IsReloading", false);
            Weapons.Add(weaponComp.WeaponType, weaponComp);

            if (weaponComp.WeaponType == WeaponType.Pistol)
            {
                weaponComp.OnShoot += () => { AS_GunSounds.PlayOneShot(S_PistolShoot); };
                weaponComp.OnReload += () => { AS_GunSounds.PlayOneShot(S_PistolReload); };
            }
            else
            {
                weaponComp.OnShoot += () => { AS_GunSounds.PlayOneShot(S_RifleShoot); };
                weaponComp.OnReload += () => { AS_GunSounds.PlayOneShot(S_RifleReload); };
            }
        }
        SetPistol();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint, 0.1f);

        //RANGO DE DETECCIÓN DEL ENEMIGO MAS CERCANO.
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_raycastOrigin, 0.1f);
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, _targetDetectionRange);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("PointPosition", transform.position + Vector3.up * 2);
        GetCloserTarget();
        if (CurrentWeapon.InfiniteBullets)
        {
            _hud._AmmoText.text = "Infinite";
            _hud._magazineText.text = "Infinite";
        }
        else
        {
            _hud._AmmoText.text = string.Format("{0} / {1}", CurrentWeapon.Magazine, CurrentWeapon.AmmoCapacity);
            _hud._magazineText.text = CurrentWeapon.backPack.ToString();
        }

        if (!lockInput)
        {
            if (!CurrentWeapon.shootPhase)
            {
                if (!CurrentWeapon.isReloading)
                {
                    if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
                        MovePlayer(_joystick.Horizontal, _joystick.Vertical);
                    else StopPlayerMovement();
                    RotatePlayer();
                }
            }
            else
            {
                StopPlayerMovement();
                RotatePlayerTowardsCloserTarget();
            } 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
           
            _movementSpeed = 0;


        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopShoot();
            _movementSpeed = 5;

        }

        if (_health <= 0)
        {
            _movementSpeed = 0;

        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0 )
        {
            MovePlayer(horizontal, vertical);
        }
       else StopPlayerMovement();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetRifle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetPistol();
        }

    }

    public void SetPistol()
    {
        CurrentWeapon = Weapons[WeaponType.Pistol];
        CurrentWeapon.gameObject.SetActive(true);
        Weapons[WeaponType.AssaultRifle].gameObject.SetActive(false);
        _anim.runtimeAnimatorController = _pistolAnimations;
    }

    public void SetRifle()
    {
        CurrentWeapon = Weapons[WeaponType.AssaultRifle];
        CurrentWeapon.gameObject.SetActive(true);
        Weapons[WeaponType.Pistol].gameObject.SetActive(false);
        _anim.runtimeAnimatorController = _rifleAnimations;
    }

    public void RotatePlayer()
    {
        if (_axisDirection != Vector3.zero)
            transform.forward = _axisDirection;
    }
    public void RotatePlayerTowardsCloserTarget()
    {
        if (_target != null)
        {
            Vector3 dirToTarget = (_target.transform.position - transform.position).normalized;
            transform.forward = dirToTarget;
        }
    }

    /// <summary>
    /// Obtiene una referencia al objetivo mas cercano.
    /// </summary>
    public void GetCloserTarget()
    {
        Collider[] targeteables = Physics.OverlapSphere(transform.position, _targetDetectionRange, _targeteables, QueryTriggerInteraction.Ignore);
        var targets = targeteables.Where(coll => 
                                   {
                                       var comp = coll.GetComponent<IFighter<HitData, HitResult>>();
                                       return (comp != null && comp.enabled);
                                   })
                                  .Select(coll => coll.GetComponent<IFighter<HitData, HitResult>>())
                                  .Where(Fighter => Fighter.gameObject != gameObject)
                                  .OrderBy(OtherFitghter => Vector3.Distance(transform.position, OtherFitghter.transform.position));

        if (targets.Any())
            _target = targets.First();

        if (_target != null)
            FindedTarget = _target.gameObject;
    }

    public void MovePlayer(float HAxis, float VAxis)
    {
        //Dirección de la cámara.
        _anim.SetBool("IsMoving", true);
        _axisDirection = _worldForward.forward * VAxis + _worldForward.right * HAxis;
        _movementDirection = _axisDirection;

        _raycastOrigin = transform.position + _axisDirection + new Vector3(0, _upOffset, 0);
        Ray ray = new Ray(_raycastOrigin, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, _groundLayer))
            _movementDirection = (hit.point - transform.position).normalized;
        hitPoint = hit.point;

        _rb.velocity = _movementDirection * _movementSpeed;
    }

    public void StopPlayerMovement()
    {
        _anim.SetBool("IsMoving", false);
        _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, 0.1f);
    }

    //Hoockeamos esto x UI.
    public void Shoot()
    {
        if (CurrentWeapon.canShoot)
        {
            CurrentWeapon.StartShooting();
            if (_target != null) RotatePlayerTowardsCloserTarget();
        }
        else
        {
            print("No puedo disparar me quede sin balas");
            //Activo un sonido de no-balas.
            AS_GunSounds.PlayOneShot(S_WithoutAmmo);
        }
        Debug.Log("player dispara");
    }
    public void StopShoot()
    {
        CurrentWeapon.StopShooting();
    }
    public void Die()
    {
        _anim.SetBool("IsDying", true);
        _hud.HideControls();
        GameProgressTracker.NotifyPlayerDied();
        lockInput = true;
    }
    //================================================ BUFFS ==============================================================================

    public void AddBullets(WeaponType weapon, int bulletAmmounts)
    {
        var myWeapon = GetWeaponByType(weapon);
   
        Debug.Log("agarre balas");
        myWeapon.sumamanual(bulletAmmounts);
    }
    public Weapon GetWeaponByType(WeaponType weaponType)
    {
        if (Weapons.ContainsKey(weaponType))
            return Weapons[weaponType];

        return null;
    }
    public void RestoreAllHealth()
    {
        Health = _maxHealth;
    }
    public void DoomGuyMode(bool active)
    {
        if (Weapons != null && Weapons.Count > 0)
        {
            foreach (var weapon in Weapons.Values)
                weapon.InfiniteBullets = active;
        }
    }
    public void AddExtraDamage(int extraDamage)
    {
        if (Weapons != null && Weapons.Count > 0)
        {
            foreach (var weapon in Weapons.Values)
                weapon.IncreaseDamage(extraDamage);
        }
    }

    //================================================ SISTEMA DE DAÑO ====================================================================

    public HitResult Hit(HitData hitData)
    {
        HitResult result = new HitResult();

        // Es mas flexible de este modo porque puedo agregar modificadores de Daño para los atques salientes
        // y Tambien resistencias para el daño entrante.
        if (hitData.Damage > 0)
        {
            Health -= hitData.Damage;
            result.Conected = true;

            if (Health <= 0) Die();
            else AS_characterSounds.PlayOneShot(S_GetHurt);
        }

        return result;
    }

    /// <summary>
    /// Permite obtener información detallada del resultado de un Hit provocado a una segunda unidad.
    /// </summary>
    /// <param name="hitResult">Resultado del Hit</param>
    public void OnHiConnected(HitResult hitResult)
    {
        //Puedo hacer cosas en base al resultado del hit.
        if (hitResult.targetEliminated)
        {
            print("Objetivo eliminado");
            _target = null;
            GetCloserTarget();
        }
    }

    /// <summary>
    /// Devuelve las estadísticas de combate de esta unidad. Modificadores de Daño.
    /// </summary>
    public HitData GetCombatStats()
    {
        return new HitData();
    }
}
