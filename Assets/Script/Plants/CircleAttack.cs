using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CircleAttack : PlantBase
{
    #region variables and fields
    private ObjectPool _pool;

    [Header("bullet amount info")]
    [SerializeField] private int _highestBulletAmount = 10;
    [SerializeField] private int _mediumBulletAmount = 8;
    [SerializeField] private int _smallestBulletAmount = 5;
    public int AmountofBullets;

    [FormerlySerializedAs("_startAngle")]
    [Header("Range of shot info")]
    public float startAngle = -90;
    [FormerlySerializedAs("_endAngle")]public float endAngle = 135;
    private bool _disappear; 
    private UIManager _UImanager;
    
    #endregion

    #region unityMethods

    protected override void Awake()
    {
        base.Awake();
        _UImanager = UIManager.Instance;
        _pool = GetComponent<ObjectPool>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        AmountofBullets = 0;
        _disappear = false;
        PlayerEntityInfo.Instance.PowerChange(5, true);
        StartCoroutine(C_AttackLoop());
    }
    #endregion

    #region other functions
    public override void WhenAtZeroWater()
    {
        if (_disappear) return;
        _disappear = true;
        UIManager.Instance.MushroomPlantInfo(-1);
        base.WhenAtZeroWater();
    }

    private void AttackMove()
    {
        switch (Age)
        {
            case State.None: AmountofBullets = 0; break;
            case State.Baby: AmountofBullets = _smallestBulletAmount; break;
            case State.Adult: AmountofBullets = _mediumBulletAmount; break;
            case State.Old: AmountofBullets = _highestBulletAmount; break;
        }

        float step = (endAngle - startAngle) / AmountofBullets;
        float angle = startAngle;

        for (int i = 0; i < AmountofBullets; i++)
        {
            float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector2 bulDir = new Vector2(dirX, dirY);

            var shot = _pool.Claim();
            shot.transform.position = transform.position;
            shot.GetComponent<Bullet>().InitBullet(bulDir, 2.6f, _UImanager.bulletStrength + 2);

            angle += step;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator C_AttackLoop()
    {
        while (Age != State.None)
        {
            if (FindObjectOfType<PlayerController>().canMove)
            {
                AttackMove();
            }
            yield return new WaitForSeconds(0.8f);
        }
    }
    #endregion
}