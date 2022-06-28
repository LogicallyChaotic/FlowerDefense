using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	private readonly int _x;
	private readonly int _y;
	private bool _isTaken;
	private GameObject _thisObject;
	private CellObjectType _cellType;
	private SpriteRenderer _sRend;
	public SpriteRenderer SRend { get => _sRend; set => _sRend = value; }
	public GameObject ThisObject { get => _thisObject; set => _thisObject = value; }
	public int X => _x;
	public int Y => _y;
	public bool isTaken { get => _isTaken; set => _isTaken = value; }
	public CellObjectType ObjectType { get => _cellType; set => _cellType = value; }
	public Direction Dir;
    public void SetRendandGameObj(SpriteRenderer sR, GameObject gO)
    {
        ThisObject = gO;
        SRend = sR;
    }

	public Cell(int x, int y)
	{
		this._x = x;
		this._y = y;
		this._cellType = CellObjectType.EMPTY;
		_isTaken = false;
	}
}
	public enum CellObjectType
	{
		EMPTY,
		PATH,
		START,
		END
	}
public enum Direction
{
	RIGHT,
	LEFT,
	UP,
	DOWN
}

