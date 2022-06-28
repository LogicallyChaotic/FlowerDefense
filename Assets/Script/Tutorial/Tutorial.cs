using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Tutorial : MonoBehaviour
{
    public int tutorialIndex;

    [FormerlySerializedAs("_movementExplain")] [Header("TutorialImages")]
    public Image movementExplain;
    [FormerlySerializedAs("_Scrollwheel")] public Image scrollwheel;
    [FormerlySerializedAs("_placingAndRemovingPlants")] public Image placingAndRemovingPlants;
    [FormerlySerializedAs("_healthBar")] public Image healthBar;
    [FormerlySerializedAs("_wateringRadiusInfo")] public Image wateringRadiusInfo;
    [FormerlySerializedAs("_typeOfPlant")] public Image typeOfPlant;
    [FormerlySerializedAs("_enemySpawn")] public Image enemySpawn;

    [FormerlySerializedAs("_wateringRadius")] public SpriteRenderer wateringRadius;
    [FormerlySerializedAs("_highlightCol")] public Color highlightCol;
    public List<KeyCode> potentialKeys;
    private Color _tempCol;


    [FormerlySerializedAs("_left")] public MapGen left;
    [FormerlySerializedAs("_right")] public MapGen right;

    private void Start()
    {
        StartCoroutine(StartTutorial());
        _tempCol = wateringRadius.color;

        FindObjectOfType<PlayerController>().canMove = false;
    }

    private void ProceedTut()
    {
        switch (tutorialIndex)
        {
            case 1:
                MovementExplain();
                break;
            case 2:
                ScrollWheel();
                break;
            case 3:
                PlacingPlants();
                break;
            case 4:
                HealthBar();
                break;
            case 5:
                WateringCircle();
                break;
            case 6:
                TypeofPlants();
                break;
            case 7:
                EnemySpawn();
                break;
            case 8:
                NextScene();
                break;
        }
    }

    private IEnumerator StartTutorial()
    {
        left.DrawPath();      
        yield return new WaitForSeconds(0.1f);
        right.DrawPath();
        tutorialIndex = 1;
        ProceedTut();
    }
    private void MovementExplain()
    {
        movementExplain.gameObject.SetActive(true);

        foreach (KeyCode key in potentialKeys.Where(key => Input.GetKeyDown(key)))
        {
            tutorialIndex = 2;
        }
    }

    private void ScrollWheel()
    {
        movementExplain.gameObject.SetActive(false);
        scrollwheel.gameObject.SetActive(true);


        if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            tutorialIndex = 3;
        }
    }

    private void PlacingPlants()
    {
        scrollwheel.gameObject.SetActive(false);
        placingAndRemovingPlants.gameObject.SetActive(true);

        if (!Input.GetMouseButtonDown(1)) return;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity);

        if (!hit) return;
        if (hit.collider.CompareTag("Plant"))
        {
            Time.timeScale = 1;
            tutorialIndex = 5;
        }

        if (hit.collider.gameObject.layer != 7) return;
        if (hit.collider.transform.childCount <= 0) return;
        if (hit.collider.transform.GetChild(0).gameObject.CompareTag("Plant"))
        {
            tutorialIndex = 5;
        }
    }

    public void NextTut()
    {

        tutorialIndex++;
        ProceedTut();
    }

    private void HealthBar()
    {
        placingAndRemovingPlants.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
    }

    private void WateringCircle()
    {
        healthBar.gameObject.SetActive(false);
        wateringRadiusInfo.gameObject.SetActive(true);
        wateringRadius.color = highlightCol;
    }

    private void TypeofPlants()
    {
        wateringRadiusInfo.gameObject.SetActive(false);
        typeOfPlant.gameObject.SetActive(true);
        wateringRadius.color = _tempCol;
    }

    private void EnemySpawn()
    {
        typeOfPlant.gameObject.SetActive(false);
        enemySpawn.gameObject.SetActive(true);
    }

    private static void NextScene()
    {
        SceneManager.LoadScene(2);
    }
}
