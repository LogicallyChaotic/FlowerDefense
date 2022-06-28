using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : Singleton<EnemySpawner>
{
    
    public int maxEnemyAmount = 3;
    public int maxWaveAmount = 2; 
    public float speedMultiplier = 0.4f;
     
    [FormerlySerializedAs("_path1")] public Sprite path1;
    [FormerlySerializedAs("_path2")] public Sprite path2;
    [FormerlySerializedAs("_overlap")] public Sprite overlap;
    [FormerlySerializedAs("_newWave")] public AudioClip newWave;
         
    [SerializeField] private List<ObjectPool> enemies;
    [SerializeField] private MapGen leftMAp;
    [SerializeField] private MapGen rightMAp;
    
    private WaveInformation _waveInfo;
    private int _maxEnemyIndex = 6;
    private int _enemyProbabilityDifference = 1;

    public int enemiesKilled;
    public int wavesSurvived;
    public int streak;
    

    private GameObject[] _tempList1;
    private GameObject[] _tempList2;

 
    
    public List<ObjectPool> pickups;
    public void Update()
    {
        if (!FindObjectOfType<PlayerController>().canMove) return;
        if(_waveInfo == null)
        {
            WaveTracker();
            enemiesKilled = 0;
        }

        if (enemiesKilled != _waveInfo.LotsofEnemies * _waveInfo.NumberofEnemies) return;
        enemiesKilled = 0;
        wavesSurvived++;

        if(wavesSurvived % 5 == 0)
        {
            _maxEnemyIndex += 3;
            _enemyProbabilityDifference++;

            maxEnemyAmount++;
            maxWaveAmount++;

            maxEnemyAmount = Mathf.Min(maxEnemyAmount, 5);
            maxWaveAmount = Mathf.Min(maxWaveAmount, 4);
        }

        if (wavesSurvived % 3 == 0)
        {
            speedMultiplier += 0.1f;
            speedMultiplier = Mathf.Min(speedMultiplier, 1);
        }

        if (wavesSurvived % 2== 0)
        {
            UIManager.Instance.DownGrades();
        }

        WaveTracker();
    }

    private void WaveTracker()
    {

        PlayerEntityInfo.Instance.PowerChange(3, false);
        _waveInfo = NewWaveInfo();
        StartCoroutine(c_WaveOrganiser());
        AudioManager.Instance.PlaySound(newWave);
    }

    private WaveInformation NewWaveInfo()
    {
        WaveInformation waveInfo = new WaveInformation(
              maxEnemyAmount,
              maxWaveAmount,
              Random.Range(0.5f, 2.5f));

        return waveInfo;

    }
    private IEnumerator c_WaveOrganiser()
    {
        yield return StartCoroutine(ChoseRoute());

        for (int i = 0; i < _waveInfo.LotsofEnemies; i++)
        {
            if (FindObjectOfType<PlayerController>().canMove)
            {
                yield return StartCoroutine(c_SpawnEnemy(_waveInfo.NumberofEnemies));
            }
        }
    }
    private IEnumerator c_SpawnEnemy(int enemiesThisRoute)
    {
        yield return new WaitForSeconds(_waveInfo.TimeBetweenEnemiesSpawning);

        for (int i = 0; i < enemiesThisRoute; i++)
        {
            if (!FindObjectOfType<PlayerController>().canMove) continue;
            GameObject[] tempList = null;
            int routIndex = Random.Range(0, 2);
            switch (routIndex)
            {
                case 0:
                    tempList = _tempList1;
                    break;
                case 1:
                    tempList = _tempList2;
                    break;

            }
            Poolable enemy = EnemyPicker().Claim();
            enemy.GetComponent<EnemyMove>().SetRoute(tempList);
            yield return new WaitForSeconds(_waveInfo.TimeBetweenEnemiesSpawning);
        }
    }

    private ObjectPool EnemyPicker()
    {
        int randNum = Random.Range(0, _maxEnemyIndex);

        if (randNum <= _enemyProbabilityDifference)
        {
            return enemies[0];
        }
        else if (randNum > _enemyProbabilityDifference && randNum <= (_enemyProbabilityDifference*2) +1)
        {
            return enemies[1];
        }
        else
        {
           return enemies[2];
        }
        
    }

    private IEnumerator ChoseRoute()
    {
        leftMAp.DrawPath();
        yield return new WaitForSeconds(1.5f);
        rightMAp.DrawPath();
        yield return new WaitForSeconds(1.5f);
        _tempList1 = leftMAp.path.ToArray();
        _tempList2 = rightMAp.path.ToArray();
        yield return new WaitForSeconds(0.2f);
    }
}
