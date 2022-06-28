using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager : Singleton<UIManager>
{
    
    #region variables and fields
    
    public Slider waterAvalible;
    [SerializeField] private Slider _potentialWaterAvalible;
    private int _plantIndex;
    private PlayerController _pC;
    private EnemySpawner _enemySpawner;

    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private Transform _wateringRadius;

    [FormerlySerializedAs("_deteriorateAmount")] [Header("values to change")]
    public float deteriorateAmount = 0.8f;
    public float regenHealth = 2f;
    public float expToAdd = 0.1f;
    public float bulletStrength;

    [Header("score")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _multipler;
    [SerializeField] private TMP_Text _waveNumber;
    [SerializeField] private TMP_Text _enemyNumber;
    public int multiplier;

    [Header("GameOver")]
    [SerializeField] private GameObject _gameOverScene;
    [SerializeField] private TMP_Text _endText;
    [SerializeField] private TMP_Text _waveEndText;
    public int score;

    [Header("pauseScreen")]
    [SerializeField] private GameObject _pauseScreen;

    [Header("buttons")]
    [SerializeField] private Button _radiusUp;
    [SerializeField] private Button _regenUp;
    [SerializeField] private Button _speedUp;
    [SerializeField] private Button _plantdeathslower;
    [SerializeField] private Button _bulletStrength;


    [Header("Audio")]
    [SerializeField] AudioClip _buttonPressed;
    [SerializeField] AudioClip _deathNoise;

    [FormerlySerializedAs("mushroom")]
    [Header("FlowerStats")]
    [SerializeField] private TMP_Text _mushroom;
    [FormerlySerializedAs("stalk")] [SerializeField] private TMP_Text _stalk;
    [FormerlySerializedAs("healing")] [SerializeField] private TMP_Text _healing;
    [SerializeField] private Cursor _cursor;

    private int _mushroomAmount;
    private int _stalkAmount;
    private int _healingAmount;
    private static readonly int Butterfly = Animator.StringToHash("butterfly");
    private static readonly int TopHat = Animator.StringToHash("topHat");
    private static readonly int Sleep = Animator.StringToHash("Sleep");
    private static readonly int Orange = Animator.StringToHash("Orange");

    #endregion
    #region unity methods
    private void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _pC = FindObjectOfType<PlayerController>();

        if (upgradeCanvas != null)
        {
            upgradeCanvas.SetActive(false);
        }

        if (_pauseScreen != null)
        {
            _pauseScreen.SetActive(false);
        }
    }


    void Update()
    {
        waterAvalible.maxValue = PlayerEntityInfo.Instance.maxPower;
        _potentialWaterAvalible.maxValue = PlayerEntityInfo.Instance.maxPower;

        _scoreText.text = score.ToString();
        _multipler.text = "x" + multiplier;

        if (EnemySpawner.Instance)
        {
            _waveNumber.text = "Wave : " + EnemySpawner.Instance.wavesSurvived;
            _enemyNumber.text = EnemySpawner.Instance.enemiesKilled + " / " + (EnemySpawner.Instance.maxEnemyAmount * EnemySpawner.Instance.maxWaveAmount);
        }

        if (_pauseScreen == null) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
        }
    }  
    
    #endregion
    
    #region other functions
    public void UpdatePotentialWater()
    {
             waterAvalible.value = PlayerEntityInfo.Instance.curPower;
             _potentialWaterAvalible.value = PlayerEntityInfo.Instance.curPower - _cursor.Amount;
             
    }
    public void ResetPlants()
    {
        _mushroomAmount = 0;
        _mushroom.text = _mushroomAmount + "/5";

        _stalkAmount = 0;
        _stalk.text = _stalkAmount + "/5";

        _healingAmount = 0;
        _healing.text = _healingAmount + "/2";
    }
    public bool MushroomPlantInfo(int addedValue)
    {
        if (addedValue < 0)
        {
            _mushroomAmount = Mathf.Max(_mushroomAmount + addedValue, 0);
            _mushroom.text = _mushroomAmount + "/5";
            return false;
        }
        else if (_mushroomAmount < 5)
        {
            _mushroomAmount += addedValue;
            _mushroom.text = _mushroomAmount + "/5";
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool StalkPlantInfo(int addedValue)
    {
       
        if (addedValue < 0)
        {
            _stalkAmount = Mathf.Max(_stalkAmount + addedValue, 0);
            _stalk.text = _stalkAmount + "/5";
            return false;
        } 
        else if (_stalkAmount < 5)
        {
            _stalkAmount += addedValue;
            _stalk.text = _stalkAmount + "/5";
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HealingPlantInfo(int addedValue)
    {
        if(addedValue < 0)
        {
            _healingAmount = Mathf.Max(_healingAmount + addedValue, 0);
            _healing.text = _healingAmount + "/2";
            return false;
        }
        else if (_healingAmount < 2)
        {
            _healingAmount += addedValue;
            _healing.text = _healingAmount + "/2";
            return true;
        }
        else
        {
            return false;
        }
    }

    public void WaterDroppedUI(Color colourToChangeTo)
    {
        waterAvalible.image.color = colourToChangeTo;
    }

    private void UpdateMultiplier()
    {
        int tempOldMultiplier = multiplier;

        if(_enemySpawner.streak <= 10)
        {
            multiplier = 2;
        }
        else if (_enemySpawner.streak <= 20)
        {
            multiplier = 3;
        }
        else if (_enemySpawner.streak <= 30)
        {
            multiplier = 4;
        }
        else if (_enemySpawner.streak <= 40)
        {
            multiplier = 5;
        }
        else if (_enemySpawner.streak <= 50)
        {
            multiplier = 6;
        }
        else if (_enemySpawner.streak > 60)
        {
            multiplier = 10;
        }

        if(multiplier != tempOldMultiplier)
        {
            _multipler.GetComponent<Animator>().SetTrigger("bonus");
        }
    }

    private void PlayButtonSound()
    {
        AudioManager.Instance.PlaySound(_buttonPressed);
    }   
    
    public void UpdatePlantIndex(int value)
    {
        _plantIndex = value;
    }

    public void PauseChangeCharacter(int characterInt)
    {
        _pC.GetComponent<Animator>().SetBool(Butterfly, false);
        _pC.GetComponent<Animator>().SetBool(TopHat, false);
        _pC.GetComponent<Animator>().SetBool(Sleep, false);
        _pC.GetComponent<Animator>().SetBool(Orange, false);

        switch (characterInt)
        {
            case 0:
                _pC.GetComponent<Animator>().SetBool(Butterfly, false);
                _pC.GetComponent<Animator>().SetBool(TopHat, false);
                _pC.GetComponent<Animator>().SetBool(Sleep, false);
                _pC.GetComponent<Animator>().SetBool(Orange, false);
                break;
            case 1:
                _pC.GetComponent<Animator>().SetBool(Butterfly, true);
                break;
            case 2:
                _pC.GetComponent<Animator>().SetBool(TopHat, true);
                break;
            case 3:
                _pC.GetComponent<Animator>().SetBool(Sleep, true);

                break;
            case 4:
                _pC.GetComponent<Animator>().SetBool(Orange, true);
                break;

        }
    }

    private void PauseMenu()
    {
        Time.timeScale = 0;
        _pC.canMove = false;
        _pauseScreen.SetActive(true);
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        PlayButtonSound();
        _pC.canMove = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        PlayButtonSound();
        _pC.canMove = true;
        _pauseScreen.SetActive(false);
    }

    public void UpSpeed()
    {
        _pC.baseMoveSpeed -= 0.7f;
    }

    public void DownGrades()
    {
        UpdateMultiplier();

        if (_wateringRadius.transform.localScale.y <= 0.75)
        {
            _radiusUp.interactable = false;
        }
        if (regenHealth <= 0.8)
        {
            _regenUp.interactable = false;
        }
        if (_pC.baseMoveSpeed <= 3.2f)
        {
            _speedUp.interactable = false;
        }
        if (deteriorateAmount >= 0.8f)
        {
            _plantdeathslower.interactable = false;
        }
        if (bulletStrength <= 0.3)
        {
            _bulletStrength.interactable = false;
        }


        if (!_bulletStrength.interactable && !_plantdeathslower.interactable && !_speedUp.interactable && !_regenUp.interactable && !_radiusUp.interactable)
        {
            return;
        }

        StartCoroutine(c_WaitforTime());
       
    }
    public void ExitUpgrades()
    {
        Time.timeScale = 1;
        PlayButtonSound();
        _pC.canMove = true;
        upgradeCanvas.SetActive(false);
        expToAdd += 0.02f;

        if(expToAdd >= 0.25)
        {
            expToAdd = 0.25f;
        }
    }

    public void GameOver()
    {
        _pC.canMove = false;
        _gameOverScene.SetActive(true);
        AudioManager.Instance.PlaySound(_deathNoise);
        StartCoroutine(PlaceScore("Your Score: "));
    }
    public void MainMenu()
    {
        Time.timeScale = 1;
        PlayButtonSound();
        _pC.canMove = true;
        SceneManager.LoadScene(0);
    }
    public void UpRadiusOfWater()
    {
        _wateringRadius.localScale = _wateringRadius.localScale * 0.8f;
    }

    public void PlantsDieSlower()
    {
        deteriorateAmount += 0.07f;
    }

    public void UpPlantingRadius()
    {
        regenHealth -= 0.2f;
    }
    public void PlantStrength()
    {
        bulletStrength -= 0.7f;
    }
    #endregion
    
    #region coroutines

    private IEnumerator c_WaitforTime()
    {
        upgradeCanvas.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        
        Time.timeScale = 0;
        _pC.canMove = false;
    }

    private IEnumerator PlaceScore(string words)
    {
        yield return new WaitForSeconds(0.4f);
        foreach (char t in words)
        {
            _endText.text += t;
            yield return new WaitForSeconds(0.05f);
        }

        _endText.text += '\n';
        _endText.text += "<size=60>";
        yield return new WaitForSeconds(0.1f);
        _endText.text += score.ToString();
        _waveEndText.text = "Waves survived: " + EnemySpawner.Instance.wavesSurvived;
        Time.timeScale = 0;
    }
#endregion
    
}
