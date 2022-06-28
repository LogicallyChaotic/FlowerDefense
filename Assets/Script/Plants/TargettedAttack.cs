using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargettedAttack : PlantBase
{
    #region private consts, variables and fields;

    private const float TIMEBETWEENSHOTS = 0.25f;
    private const float MINIMUMDISTANCE = 2.5f;
    private const float MEDIUMDISTANCE = 3f;
    private const float MAXIMUMDISTANCE = 3.5f;
    private int _maxNumbOfEnemies = 1;
    private float _distanceToShoot = MINIMUMDISTANCE; 
    private bool _canShoot = true;
    private bool _disappear;
    private ObjectPool _pool;

    [Header("bullet info")]
    [SerializeField] private float _strength;
    [SerializeField] private LayerMask _enemyLayer;

    #endregion

    #region unitymethods
    protected override void Awake()
    {
        base.Awake(); 
        _pool = GetComponent<ObjectPool>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _canShoot = true;
        _disappear = false;

        PlayerEntityInfo.Instance.PowerChange(7, true);
    }

    private void Update()
    {
        var results = new Collider2D[3];
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, _distanceToShoot,results, _enemyLayer);
        _strength = UIManager.Instance.bulletStrength;

        if (size <= 0) return;
        switch (Age)
        {
            case State.None: break;
            case State.Baby:  _maxNumbOfEnemies = 1; _distanceToShoot = MINIMUMDISTANCE; break;
            case State.Adult: _maxNumbOfEnemies = 2; _distanceToShoot = MEDIUMDISTANCE; break;
            case State.Old: _maxNumbOfEnemies = 3; _distanceToShoot = MAXIMUMDISTANCE; break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        for (int i = 0; i < _maxNumbOfEnemies; i++)
        {
            if (!_canShoot) continue;
            if (results.Length > 0)
            {
                StartCoroutine(c_Shoot(results[i].gameObject));
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.15f, 0.47f);
        Gizmos.DrawWireSphere(transform.position, 3);
    }
    #endregion

    #region other functions
    /// <summary>
    /// shoot bullets targeted at the nearest enemy in range.
    /// </summary>
    /// <param name="obj">  the other gameobject collided with</param>
    private void Shoot(GameObject obj)
    {
        if (obj == null) return;
        Poolable tempBulletPool = _pool.Claim();
        Vector3 position = transform.position;
        tempBulletPool.transform.position = position;
        Bullet tempBul = tempBulletPool.GetComponent<Bullet>();
        Vector3 tempAim = (obj.transform.position - position).normalized;
        tempBul.InitBullet(tempAim, 4.6f, _strength);
    }
    public override void WhenAtZeroWater()
    {
        switch (_disappear)
        {
            case false:
                _disappear = true;
                UIManager.Instance.StalkPlantInfo(-1);
                base.WhenAtZeroWater();
                break;
        }
    }
    #endregion

    #region coroutines

    private IEnumerator c_Shoot(GameObject obj)
    {
        _canShoot = false;
        yield return new WaitForSeconds(TIMEBETWEENSHOTS);
        Shoot(obj);
        _canShoot = true;
    }
    #endregion
}
