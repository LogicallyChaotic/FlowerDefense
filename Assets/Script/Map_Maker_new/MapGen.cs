using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapGen : MonoBehaviour
{
    public Sprite grassSepSprites;
    public Sprite portal;
    public Sprite pathTile;
    public Color col;

    [Range(3, 20)]
    public int width, length;

    public int widthMin;
    [FormerlySerializedAs("LengthMin")] public int lengthMin;
    public int widthMax;
    [FormerlySerializedAs("LengthMax")] public int lengthMax;
    private Grid _grid;
    private Vector3 _startPosition;
    private Cell _oldCell;
    private int _currWidthAmount;
    private int _currHeightAmount;
    private int _tempNum;

    [FormerlySerializedAs("Path")] public List<GameObject> path;
    public void Start()
    {
        _grid = new Grid(width, length, null, col, this.gameObject, this.transform.position);
    }
    public void DrawPath()
    {
        StartCoroutine(ResetPath());
    }
    private void ChoseStartPosition()
    {
        _startPosition = RandomlyChoosePositionOnTheEdgeOfTheGrid(_grid, _startPosition);
        _grid.SetCell((int)_startPosition.x, (int)_startPosition.y, CellObjectType.START);

        GameObject g = _grid.ReturnPoint((int)_startPosition.x, (int)_startPosition.y);
        Cell tempCell = _grid.ReturnCell((int)_startPosition.x, (int)_startPosition.y);
        tempCell.SRend.sprite = portal;
        tempCell.SetRendandGameObj(tempCell.SRend, g);

        path.Add(g);
        _oldCell = NewPoint(_startPosition += Vector3.down);

        CheckPath(_startPosition, true);
    }
    private IEnumerator ResetPath()
    {
        foreach (GameObject tile in path)
        {
            yield return new WaitForSeconds(0.02f);
            tile.gameObject.layer = 8;
            tile.GetComponent<SpriteRenderer>().sprite = null;
        }
        foreach (PlantBase plant in FindObjectsOfType<PlantBase>())
        {
            plant.NewRoute();
            yield return new WaitForSeconds(0.2f);
            UIManager.Instance.ResetPlants();
            plant.transform.parent.gameObject.layer = 8;
            plant.GetComponent<Poolable>().Release();
        }
        path.Clear();

        ChoseStartPosition();
    }
    private Cell NewPoint(Vector3 startPosition)
    {
        GameObject g = _grid.ReturnPoint((int)startPosition.x, (int)startPosition.y);
        g.layer = 7;
        Cell tempCell = _grid.ReturnCell((int)startPosition.x, (int)startPosition.y);
        tempCell.SRend.sprite = pathTile;
        tempCell.SetRendandGameObj(tempCell.SRend, g);

        path.Add(g);

        return tempCell;
    }
    private Vector3 RandomlyChoosePositionOnTheEdgeOfTheGrid(Grid grid, Vector3 startPosition)
    {
        Vector3 position;
        do
        {
            position = new Vector3(Random.Range(0, grid.Width), grid.Length - 1, 0);
        } while (Vector3.Distance(position, startPosition) <= 1);

        return position;
    }
    private void CheckPath(Vector3 prevPos, bool random)
    {
        if (random)
        {
            _tempNum = Random.Range(0, 3);
        }

        Vector3 curPos;
        Cell tempCell;

        switch (_tempNum)
        {
            case 0:
                //right
                curPos = prevPos + Vector3.right;
                if (_grid.DoesCellExist(curPos.x, curPos.y))
                {
                    tempCell = NewPoint(curPos);
                    tempCell.Dir = Direction.RIGHT;
                    _oldCell = tempCell;
                    _currWidthAmount++;

                    StartCoroutine(CheckIfValid(curPos, _currWidthAmount, 0, Direction.RIGHT,false));
                }
                else
                {
                    CheckPreviousCell(prevPos);
                }
                break;

            case 1:
                //left
                curPos = prevPos + Vector3.left;
                if (_grid.DoesCellExist(curPos.x, curPos.y))
                {
                    tempCell = NewPoint(curPos);
                    tempCell.Dir = Direction.LEFT;
                    _oldCell = tempCell;
                    _currWidthAmount++;

                    StartCoroutine(CheckIfValid(curPos, _currWidthAmount, 1,Direction.LEFT,false));
                }
                else
                {
                    CheckPreviousCell(prevPos);
                }
                break;
            case 2:
                //down
                curPos = prevPos + Vector3.down;
                if (_grid.DoesCellExist(curPos.x, curPos.y))
                {
                    tempCell = NewPoint(curPos);
                    tempCell.Dir = Direction.DOWN;
                    _oldCell = tempCell;
                    _currHeightAmount++;

                    StartCoroutine(CheckIfValid(curPos, _currHeightAmount, 2,Direction.DOWN, true));
                }
                break;
        }
    }

    private void CheckPreviousCell(Vector3 curPos)
    {
        if (_grid.EndPoints.Contains(_grid.ReturnCell((int) curPos.x, (int) curPos.y))) return;
        switch (_oldCell.Dir)
        {
            case Direction.RIGHT:
                _tempNum = 2;
                break;

            case Direction.LEFT:
                _tempNum = 2;
                break;

            case Direction.DOWN:

                int tempRandProb = Random.Range(0, 2);
                _tempNum = tempRandProb switch
                {
                    0 => 0,
                    1 => 1,
                    _ => _tempNum
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        CheckPath(curPos, false);
    }

    private IEnumerator CheckIfValid(Vector3 currentPos, int indexToIncrease, int indexNum, Direction dir, bool isHeight)
    {
        yield return new WaitForSeconds(0.01f);

        if (_grid.EndPoints.Contains(_grid.ReturnCell((int) currentPos.x, (int) currentPos.y))) yield break;
        int tempMax;
        int tempMin;

        if(isHeight)
        {
            tempMax = lengthMax;
            tempMin = lengthMin;
        }
        else
        {
            tempMax = widthMax;
            tempMin = widthMin;
        }


        if (indexToIncrease < tempMin)
        {
            _tempNum = indexNum;

            if (isHeight)
            {
                _currHeightAmount++;
            }
            else
            {
                _currWidthAmount++;

            }


            CheckPath(currentPos, false);
        }
        else
        {
            int tempProb;
            tempProb = indexToIncrease < tempMax ? Random.Range(0, 2) : 0;

            if (tempProb < 1)
            {
                if (isHeight)
                {
                    _currHeightAmount = 0;
                }
                else
                {
                    _currWidthAmount = 0;

                }

                if (dir == Direction.RIGHT || dir == Direction.LEFT)
                {
                    _tempNum = 2;
                    CheckPath(currentPos, false);
                }
                else
                {
                    int tempRandProb = Random.Range(0, 2);

                    if (tempRandProb < 1)
                    {
                        _tempNum = 0;
                        CheckPath(currentPos, false);
                    }
                    else
                    {
                        _tempNum = 1;
                        CheckPath(currentPos, false);
                    }
                }
            }
            else
            {
                _tempNum = indexNum;
                if (isHeight)
                {
                    _currHeightAmount++;
                }
                else
                {
                    _currWidthAmount++;

                }
                CheckPath(currentPos, false);
            }
        }
    }
}