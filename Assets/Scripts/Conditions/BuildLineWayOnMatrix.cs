using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatrixForConditions.BlockAtMatrix;

public class BuildLineWayOnMatrix : MonoBehaviour
{
    private enum Directions
    {
        UpDown,
        LeftRight
    }
    
    private Vector3 _direction;
    private MatrixForConditions _matrix;
    private int _id;
    private int _x, _y;
    private int _blockedX = 0, _blockedY = 0;
    private Vector3 _zAxis;
    private Vector3 _yAxis;
    private Directions _dir;
    
    private void Start()
    {
        _zAxis = transform.parent.forward;
        _yAxis = transform.parent.up;
    }

    private void OnEnable()
    {
        LineController.singleton.onStartGame += Reset;
        LineController.singleton.onCancelGame += Reset;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= Reset;
        LineController.singleton.onCancelGame -= Reset;
    }

    public void Init(int id, MatrixForConditions matrix, (int, int) posAtMatrix)
    {
        _id = id;
        _matrix = matrix;
        (_x, _y) = _matrix.ConvertXY(posAtMatrix.Item1, posAtMatrix.Item2);
        _x -= 1;
        _y -= 1;
    }
    
    public void BlockTheWay(LineRenderer lineRenderer)
    {
        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 finishPoint = lineRenderer.GetPosition(1);
        _direction = finishPoint - startPoint;
        float dotZ = Vector3.Dot(_direction.normalized, _zAxis.normalized);
        float dotY = Vector3.Dot(_direction.normalized, _yAxis.normalized);
        if (dotZ > 0.7f)
        {
            _dir = Directions.LeftRight;
            _blockedX = _x - 1;
            _blockedY = _y;
        }
        else if (dotZ < -0.7f)
        {
            _dir = Directions.LeftRight;
            _blockedX = _x + 1;
            _blockedY = _y;
            
        }
        else if (dotY > 0.7f)
        {
            _dir = Directions.UpDown;
            _blockedX = _x;
            _blockedY = _y - 1;
        }
        else
        {
            _dir = Directions.UpDown;
            _blockedX = _x;
            _blockedY = _y + 1;
        }
        
        Block(_blockedX, _blockedY, BlockType.Blocked, _dir);
    }

    private void Block(int x, int y, BlockType type, Directions dir)
    {
        if (dir == Directions.LeftRight)
        {
            for (int i = -1; i < 2; i++)
            {
                _matrix.SetBlockType(x + i, y, type);
            }
        }
        else
        {
            for (int i = -1; i < 2; i++)
            {
                _matrix.SetBlockType(x, y + i, type);
            }
        }
    }

    private void Reset(int id)
    {
        if (id == _id && _blockedX + _blockedY != 0)
        {
            Block(_blockedX, _blockedY, BlockType.Empty, _dir);
        }
    }    
    
    public void Reset()
    {
        if (_blockedX + _blockedY != 0)
        {
            Block(_blockedX, _blockedY, BlockType.Empty, _dir);
        }
    }
}
