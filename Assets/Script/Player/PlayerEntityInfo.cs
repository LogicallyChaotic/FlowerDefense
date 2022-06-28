using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerEntityInfo : Singleton<PlayerEntityInfo>,IPower
{
    #region variables & fields
    
    [FormerlySerializedAs("_possiblePower")] public float possiblePower;
    [FormerlySerializedAs("watermask")] public LayerMask waterMask;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private float _maxRegen;
    [SerializeField] private float _minRegen;
    [SerializeField] private float _maximumPower;
    [FormerlySerializedAs("_red")] public Color red;
    [FormerlySerializedAs("_normalBar")] public Color normalBar;
    
    private float _powerRegen;
    private bool _gameOverTriggered;
    private Animator _mainCameraAnimator;
    private static readonly int Shake = Animator.StringToHash("Shake");

    public float maxPower { 
        get => _maximumPower;
        set
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            possiblePower = value;
            maxPower = _maximumPower;
        }
    }
     public bool isAlive
    {
        get => curPower > 0;
        set { }
    }
    public float curPower { get;  set; }

    #endregion

    #region UnityMethods

    private void Start()
    {
        _mainCameraAnimator = Camera.main.GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        _powerRegen = UIManager.Instance.regenHealth;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.gameObject.layer == waterMask)
       {
           _powerRegen = _maxRegen;
       }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == waterMask)
        {
            _powerRegen = _minRegen;
        }
    }


    #endregion
    #region Other functions

    public void InitPowerAmount()
    {
        _powerRegen = _minRegen;
        curPower = maxPower;
    }
    public void Update()
    {
        if (isAlive) return;
        if (_gameOverTriggered != false) return;
        _gameOverTriggered = true;
        StartCoroutine(c_GameOver());
    }

    public void LateUpdate()
    {
        if (isAlive)
        {
            PowerChange(_powerRegen * Time.deltaTime, false);
        }
    }
    /// <summary>
    /// Effect the power level if the player is still alive
    /// </summary>
    /// <param name="powerChange">amount to change power by, always positive</param> 
    /// <param name="Taken"> whether its taken from the total power or added, true if taken</param>
    public void PowerChange(float powerChange, bool Taken)
    {
        if (!isAlive) return;
        if (Taken)
        {
                
            curPower -= powerChange;
        }
        else
        {
            curPower += powerChange;
            curPower = Mathf.Min(curPower, maxPower);

        }

        UIManager.Instance.UpdatePotentialWater();
    }

    private IEnumerator c_GameOver()
    {
        Time.timeScale = 0.6f;
        HurtEffect();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.GameOver();
    }
    public void HurtEffect()
    {
        _hitEffect.Emit(10);
        _mainCameraAnimator.SetTrigger(Shake);
    }
    #endregion


}
