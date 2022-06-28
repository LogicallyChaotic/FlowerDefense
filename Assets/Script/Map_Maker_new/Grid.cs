using System.Collections.Generic;
using UnityEngine;

public class Grid
{
	private readonly int _width;
	private readonly int _length;
	private Cell[,] _cellGrid;
	public int Width => _width;
	public int Length => _length;
	public readonly List<Cell> EndPoints = new List<Cell>();
	public Grid(int width, int length, Sprite grassSepSprites, Color col, GameObject parentObj, Vector3 startPos)
	{
		this._width = width;
		this._length = length;
		CreateGrid(grassSepSprites,col, parentObj,startPos);
	}

	private void CreateGrid(Sprite grassSepSprites, Color colour, GameObject parentObj, Vector3 startPos)
	{
		_cellGrid = new Cell[_length, _width];
		for (int row = 0; row < _length; row++)
		{
			for (int col = 0; col < _width; col++)
			{
				_cellGrid[row, col] = new Cell(col, row);
                GameObject g = new GameObject("X: " + (col+ startPos.x) + "Y: " + (row + startPos.y));
                _cellGrid[row, col].SetRendandGameObj(g.AddComponent<SpriteRenderer>(), g);
                g.transform.position = new Vector3(col,row, 0) + startPos;
				g.transform.parent = parentObj.transform; 
				SpriteRenderer sR = _cellGrid[row, col].SRend;
                sR.sprite = grassSepSprites;
                sR.color = colour;

				g.AddComponent<BoxCollider2D>();
				g.GetComponent<BoxCollider2D>().isTrigger = true;
				g.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
				g.gameObject.layer = 8;

                _cellGrid[row, col].SetRendandGameObj(sR, g);

				if(row == 0)
                {
					EndPoints.Add(_cellGrid[col, row]);
                }

			}
		}
	}
	/// <summary>
	/// set the cell's object type
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="objectType"></param>the cell object type
	/// <param name="isMadeEmpty"></param>false if the object type is empty
	public void SetCell(int x, int y, CellObjectType objectType, bool isMadeEmpty = false)
	{
		_cellGrid[y, x].ObjectType = objectType;
		_cellGrid[y, x].isTaken = isMadeEmpty;
	}

	/// <summary>
	/// check if the cell is empty or not (the cell object type)
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool IsCellTaken(int x, int y)
	{
		return _cellGrid[y, x].isTaken;
	}

	/// <summary>
	/// calculates the index for this position
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public int CalculateIndexFromCoordinates(int x, int y)
	{
		return x + y * _width;
	}

	public Vector3 CalculateCoordinatesFromIndex(int randomIndex)
	{
		int x = randomIndex % _width;
		int y = randomIndex / _width;
		return new Vector3(x, y, 0);
	}

	/// <summary>
	/// check that this cell exits and return a bool
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool DoesCellExist(float x, float y)
	{
		return !(x >= _width) && !(x < 0) && !(y >= _length) && !(y < 0);
	}

    public GameObject ReturnPoint(int x, int y)
    {
       return _cellGrid[y, x].ThisObject;
    }
    public SpriteRenderer ChangeSprite(Sprite sprite, int x, int y)
    {
        _cellGrid[y, x].SRend.sprite = sprite;
        return _cellGrid[y, x].SRend;
    }
	/// <summary>
	/// gets the cell and returns it 
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public Cell ReturnCell(int x, int y)
	{
		return DoesCellExist(x, y) == false ? null : _cellGrid[y, x];
	}
}
