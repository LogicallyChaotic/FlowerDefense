using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerController : Singleton<PlayerController>
{ 
    #region fields and variables;
    private Vector2 _moveDir;
    private float _moveSpeed;
    [FormerlySerializedAs("_baseMoveSpeed")]
    
    [Header("Movement")]
    [Range(0, 10)]
    public float baseMoveSpeed;
    public bool canMove;
    
    private Rigidbody2D _rB;
    private Animator _anim;
    private static readonly int Speed = Animator.StringToHash("Speed");

    #endregion

    #region UnityMethods
    protected override void Awake()
    {
        base.Awake();
        _rB = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }
    public void Start()
    {
        PlayerEntityInfo.Instance.InitPowerAmount();
    }

    void Update()
    {
        if (canMove)
        {
            ProcessInputs();
            MovePlayer();
        }
    }

    #endregion
    #region other functions
    void ProcessInputs()
    {
        _moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _moveSpeed = Mathf.Clamp(_moveDir.magnitude, 0.0f, 1.0f);
        _anim.SetFloat(Speed, _moveSpeed);
        _anim.speed =_moveSpeed;
        _moveDir.Normalize();

        GetComponent<SpriteRenderer>().flipX = Input.GetAxis("Horizontal") > 0;
    }

    private void MovePlayer()
    {
        if (canMove)
        {
            _rB.velocity = _moveDir * _moveSpeed * baseMoveSpeed;
        }
    }
    #endregion
}