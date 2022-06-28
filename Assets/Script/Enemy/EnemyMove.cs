using UnityEngine;

public class EnemyMove : MonoBehaviour
{
   [HideInInspector] public GameObject[] wayPointArray;
   [HideInInspector] public int currentWayPoint;
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private AudioClip _endofroute;
    
    private GameObject _targetWayPoint;
    private bool _startWalking;
    private EnemySpawner _enemySpawner;
    private float _currentSpeed;
    
    private void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }
    private void OnEnable()
    {
        currentWayPoint = 0;
        _currentSpeed = EnemySpawner.Instance.speedMultiplier * _speed;
    }
    void Update()
    {
        if (!_startWalking) return;
        if (currentWayPoint < wayPointArray.Length)
        {
            if (_targetWayPoint == null)
                _targetWayPoint = wayPointArray[currentWayPoint];

            if (!GetComponent<EnemyEntity>().sendDeath)
            {
                Walk();
            }
        }
        else
        {
            _startWalking = false;
            currentWayPoint = 0;
            _targetWayPoint = wayPointArray[0];
        }
    }
    private void Walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetWayPoint.transform.position, _currentSpeed * Time.deltaTime);

        if (transform.position != _targetWayPoint.transform.position) return;
        if (currentWayPoint >= wayPointArray.Length) return;
        currentWayPoint++;
        if (currentWayPoint < wayPointArray.Length)
        {
            _targetWayPoint = wayPointArray[currentWayPoint];
        }
        else
        {
            _enemySpawner.enemiesKilled++;
            _enemySpawner.streak = 1;
            PlayerEntityInfo.Instance.PowerChange(30, true);
            AudioManager.Instance.PlaySound(_endofroute);
            PlayerEntityInfo.Instance.HurtEffect();
            GetComponent<Poolable>().Release();
        }
    }
    public void SetRoute(GameObject[] tempWayPointArray)
    {
        currentWayPoint = 0;
        wayPointArray = tempWayPointArray;
        transform.position = tempWayPointArray[0].transform.position;
        _targetWayPoint = wayPointArray[0];
        _startWalking = true;
    }
}
