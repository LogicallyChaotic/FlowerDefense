using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyEntity : MonoBehaviour, IPower
{
    #region variables and fields 
    public EnemyType thisEnemyType; 
    public float hitPoints;
    [HideInInspector]public bool sendDeath; 

    [FormerlySerializedAs("deathParticles")]
    [Header("Death variables")]
    [SerializeField] private ParticleSystem _deathParticles;
    [FormerlySerializedAs("scoreWhenKilled")] [SerializeField] private int _scoreWhenKilled;
    [FormerlySerializedAs("damage")] [SerializeField] private float _damage;

    [Header ("Audio sources")]
    [FormerlySerializedAs("enemyHit")][SerializeField] private AudioClip _enemyHit;
    [FormerlySerializedAs("enemyDie")] [SerializeField] private AudioClip _enemyDie;
    [FormerlySerializedAs("enemyHitPlayer")] [SerializeField] private AudioClip _enemyHitPlayer;

    private Poolable _poolable;
    private EnemySpawner _enemySpawner;
    private Animator _anim;
    private SpriteRenderer _spriteRend;
    private AudioManager _audioManager;
    
    private const float MAXIMUMPOWER = 0;
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Round = Animator.StringToHash("Round");
    private static readonly int Square = Animator.StringToHash("Square");
    private static readonly int Triangle = Animator.StringToHash("Triangle");
    private static readonly int Power = Shader.PropertyToID("_Power");

    public enum EnemyType
    {
        ROUND,
        SQUARE,
        TRIANGLE
    }
    public float maxPower
    {
        get => MAXIMUMPOWER;
        set
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            hitPoints = value;
            maxPower = MAXIMUMPOWER;
        }
    }
    public float curPower { get; set; }
    public bool isAlive
    {
        get => curPower < 1;
        set { }
    }
    #endregion
    #region unity methods 
    private void Awake()
    {
        _audioManager = AudioManager.Instance;
        _poolable = GetComponent<Poolable>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _anim = GetComponent<Animator>();
        _spriteRend = GetComponent<SpriteRenderer>();
    }
    public void OnEnable()
    {
        InitPowerAmount();
        sendDeath = false;

        switch (thisEnemyType)
        {
            case EnemyType.ROUND:
                _anim.SetBool(Round, true);
                break;
            case EnemyType.SQUARE:
                _anim.SetBool(Square, true);
                break;
            case EnemyType.TRIANGLE:
                _anim.SetBool(Triangle, true);
                break;
        }
    }
    public void Start()
    {
        InitPowerAmount();
    }
    public void Update()
    {
        _spriteRend.material.SetFloat(Power, curPower);

        if (!isAlive)
        {
            StartCoroutine(c_Dead());
        }
    }

    #endregion
    #region other functions and methods

    private IEnumerator c_Dead()
    {
        if (!FindObjectOfType<PlayerController>().canMove) yield break;

        if (sendDeath) yield break;
        
        _deathParticles.Emit(10);
        UpgradeDrop();

        FindObjectOfType<EnemySpawner>().streak++;
        int tempScore = _scoreWhenKilled * UIManager.Instance.multiplier;
        UIManager.Instance.score += tempScore;

        sendDeath = true;

        _anim.SetTrigger(Dead);
        _audioManager.PlaySound(_enemyDie);
        yield return new WaitForSeconds(0.4f);
        _enemySpawner.enemiesKilled++;
        _poolable.Release();
    }
    public void PowerChange(float hitAmount, bool taken)
    {
        if (taken)
        {
            curPower += hitPoints * hitAmount;
        }
        else
        {
            curPower -= hitPoints * hitAmount;
        }
    }

    private void UpgradeDrop()
    {
        int randNum = Random.Range(0, 11);

        if (randNum > 1) return;
        
        Poolable droppedItem = EnemySpawner.Instance.pickups[Random.Range(0, EnemySpawner.Instance.pickups.Count)].Claim();
        droppedItem.transform.position = this.transform.position;
    }

    public void InitPowerAmount()
    {
        curPower = maxPower;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            PowerChange(collision.gameObject.GetComponent<Bullet>().Damage, true);
            _audioManager.PlaySound(_enemyHit);
        }

        if (!collision.gameObject.CompareTag("Player")) return;
        
        PlayerEntityInfo.Instance.PowerChange(_damage, true);
        _audioManager.PlaySound(_enemyHitPlayer);
    }
    #endregion
}