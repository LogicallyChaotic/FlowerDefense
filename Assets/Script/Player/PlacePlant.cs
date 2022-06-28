using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePlant : MonoBehaviour
{
    #region fields and variables;

    [Header("Water Mechanic")]

    public int plantindex;
    [SerializeField] private List<ObjectPool> _plantPools;
    [SerializeField] private List<GameObject> _plants;
    [SerializeField] private LayerMask _plantingLayer;
    private PlayerController _player;
    private GameObject _curPlant;

    [SerializeField] private AudioClip _mushroomPlaced;
    [SerializeField] private AudioClip _stalkplaced;
    [SerializeField] private AudioClip _teleportPlaced;

    [SerializeField] private AudioClip _plantTaken;

    #endregion

    #region unity methods
    public void Awake()
    {
        _player = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (!GetComponent<PlayerController>().canMove) return;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            plantindex = 0;
            _curPlant = _plants[plantindex];

            UIManager.Instance.UpdatePlantIndex(plantindex);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            plantindex = 1;
            _curPlant = _plants[plantindex];

            UIManager.Instance.UpdatePlantIndex(plantindex);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            plantindex = 2;
            _curPlant = _plants[plantindex];

            UIManager.Instance.UpdatePlantIndex(plantindex);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            plantindex++;

            if (plantindex >= _plants.Count)
            {
                plantindex = 0;
            }

            _curPlant = _plants[plantindex];

            UIManager.Instance.UpdatePlantIndex(plantindex);
        }

        //use the LEFT mouse button to deploy a plant;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(worldPoint.x, worldPoint.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity);

            if (hit)
            {
                if (hit.collider.gameObject.layer == 8)
                {

                    PlantFlower(hit.collider.transform);
                    hit.collider.gameObject.layer = 7;
                }
            }
        }

        if (!Input.GetMouseButtonDown(1)) return;
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(worldPoint.x, worldPoint.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity);

            if (!hit) return;
            if (hit.collider.gameObject.GetComponent<PlantBase>())
            {
                RemovePlant(hit.collider.gameObject);
            }
                    
            else if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.transform.childCount > 0)
                {
                    RemovePlant(hit.collider.transform.GetChild(0).gameObject);
                }
            }
        }
    }
#endregion

private void PlantFlower(Transform transform)
    {

        if (UpdateHUD(plantindex, 1))
        {
            Poolable plant = _plantPools[plantindex].Claim();

            switch (plantindex)
            {
                case 0:
                    AudioManager.Instance.PlaySound(_mushroomPlaced);

                    break;
                case 1:
                    AudioManager.Instance.PlaySound(_stalkplaced);

                    break;
                case 2:
                    AudioManager.Instance.PlaySound(_teleportPlaced);

                    break;
                default:
                    break;
            }

            plant.transform.parent = transform.transform;
            plant.transform.localPosition = Vector3.zero;
        }
        else
        {
            //can't plant plant!
        }
    }

private bool UpdateHUD(int plantNum, int numToAdd)
    {
        switch (plantNum)
        {
            case 0:
                return UIManager.Instance.MushroomPlantInfo(numToAdd);
            case 1:
                return UIManager.Instance.StalkPlantInfo(numToAdd);
            case 2:
                return UIManager .Instance.HealingPlantInfo(numToAdd);
        }
        return false;
    }

private void RemovePlant(GameObject hit)
    {
        PlayerEntityInfo.Instance.PowerChange(2, false);
        hit.gameObject.GetComponent<PlantBase>().WhenAtZeroWater();
        AudioManager.Instance.PlaySound(_plantTaken);
    }
}

