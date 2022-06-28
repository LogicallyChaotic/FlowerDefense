using System.Collections;
using UnityEngine;
public class PlantBase : MonoBehaviour
{
    #region variables and fields
    [Header ("watering information")]
     public float powerTakenToGrow;
     private Animator _anim;
    private float _waterChangeAmount;
    private float _waterHolding;
    private bool _watering;

    [Header("plant state info ")]
    [SerializeField]private Sprite baby;
    [SerializeField]private Sprite adult;
    [SerializeField]private Sprite old;

    protected enum State
    {
        None, Baby, Adult, Old
    }
    protected State Age;
    private State _prevAge;
    protected SpriteRenderer SpriteRend;
    [SerializeField] protected ParticleSystem growth;
    private static readonly int Water = Animator.StringToHash("Water");

    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        _prevAge = State.Baby;
         _anim = GetComponent<Animator>();
        SpriteRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnEnable()
    {
        if (Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }

        _watering = true;
        _waterHolding = 1.5f;
        UpdateAge();
        _waterChangeAmount = -UIManager.Instance.deteriorateAmount;
        StartCoroutine(C_Growth());
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Water")) return;
        _waterChangeAmount = 1;
        _anim.SetBool(Water, true);
        StartCoroutine(C_Water());
    }
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            _waterChangeAmount = -UIManager.Instance.deteriorateAmount;
            _anim.SetBool(Water, false);
            StopCoroutine(C_Water());
        }
    }
    #endregion

    #region other methods and functions   
    protected virtual void Growth()
    {
        _waterHolding += _waterChangeAmount;
    }

    protected virtual void UpdateAge()
    {
        if (_waterHolding <= 0)
        {
            WhenAtZeroWater();
            _watering = false;
        }
        else if (_waterHolding < 3)
        {
            Age = State.Baby;
            SpriteRend.sprite = baby;
            _watering = true;
        }
        else if (_waterHolding > 3 && _waterHolding < 5.5)
        {
            Age = State.Adult;
            SpriteRend.sprite = adult;
            _watering = true;
        }
        else if (_waterHolding > 5.5)
        {
            Age = State.Old;
            SpriteRend.sprite = old;
            _watering = true;
            _waterHolding = Mathf.Max(_waterHolding, 9);
        }

        if(_prevAge != Age)
        {
            growth.Emit(10);
        }
        _prevAge = Age;
    }

    public virtual void NewRoute()
    {
        growth.Emit(10);
        PlayerEntityInfo.Instance.PowerChange(5, false);
    }
    public virtual void WhenAtZeroWater()
    {
        Age = State.None;
        _waterHolding = 0;
        this.transform.parent.gameObject.layer = 8;
        this.gameObject.GetComponent<Poolable>().Release();
    }
    #endregion

    #region coroutines
    private IEnumerator C_Growth()
    {
        while(_watering)
        {
            yield return new WaitForSeconds(1f);
            UpdateAge();
            Growth();
        }
    }

    private IEnumerator C_Water()
    {
        while (_waterChangeAmount == 1)
        {
            yield return new WaitForSeconds(1f);
            PlayerEntityInfo.Instance.PowerChange(powerTakenToGrow/1.2f, true);
        }
    }
    #endregion
}