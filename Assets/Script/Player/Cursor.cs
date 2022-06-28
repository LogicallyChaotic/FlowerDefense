using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private List<Sprite> _plants;
    [SerializeField] private List<Vector3> _plantRange;
    [SerializeField] private List<int> _costs;
    private Sprite _curPlant;
    private Vector3 _curRange;
    private int _plantindex;
    public int Amount;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = _curPlant;
        transform.GetChild(0).transform.localScale = _curRange;

        _plantindex = 0;
        _curPlant = _plants[_plantindex];
        _curRange = _plantRange[_plantindex];

        UIManager.Instance.UpdatePlantIndex(_plantindex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(mousePosition.x - 0.2f, mousePosition.y - 0.2f);
        }

        GetComponent<SpriteRenderer>().sprite = _curPlant;
        transform.GetChild(0).transform.localScale = _curRange;


        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            _plantindex = 0;
            _curPlant = _plants[_plantindex];
            _curRange = _plantRange[_plantindex];
            Amount = _costs[_plantindex];

            UIManager.Instance.UpdatePlantIndex(_plantindex);

            UIManager.Instance.UpdatePotentialWater();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            _plantindex = 1;
            _curPlant = _plants[_plantindex];
            _curRange = _plantRange[_plantindex];
            Amount = _costs[_plantindex];
            UIManager.Instance.UpdatePlantIndex(_plantindex);

            UIManager.Instance.UpdatePotentialWater();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            _plantindex = 2;
            _curPlant = _plants[_plantindex];
            _curRange = _plantRange[_plantindex];
            Amount = _costs[_plantindex];
            UIManager.Instance.UpdatePlantIndex(_plantindex);

            UIManager.Instance.UpdatePotentialWater();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            _plantindex++;

            if (_plantindex >= _plants.Count)
            {
                _plantindex = 0;
            }

            _curPlant = _plants[_plantindex];
            _curRange = _plantRange[_plantindex];
            Amount = _costs[_plantindex];

            UIManager.Instance.UpdatePotentialWater();
            UIManager.Instance.UpdatePlantIndex(_plantindex);
        }
    }
}
