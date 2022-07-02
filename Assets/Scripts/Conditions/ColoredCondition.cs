using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatrixForConditions.BlockAtMatrix;

public abstract class ColoredCondition : Condition
{
    protected Color ownBlockColor;
    private MatrixForConditions _matrix;
    private Dictionary<(int, int), MatrixForConditions.BlockAtMatrix> _blockDict;
    private Queue<(int, int)> _positionsQueue;
    private int _x;
    private int _y;

    public void SetOwnBlockColor(Color color)
    {
        ownBlockColor = color;
    }

    public void SetMatrixOfConditions(MatrixForConditions matrix)
    {
        _matrix = matrix;
        _blockDict = new Dictionary<(int, int), MatrixForConditions.BlockAtMatrix>(_matrix.Length);
        _positionsQueue = new Queue<(int, int)>(_matrix.Length);
    }
    
    public override bool CheckCondition()
    {
        _positionsQueue.Enqueue((_x, _y));
        _blockDict.Add((_x, _y), _matrix[_x, _y]);
        
        while (_positionsQueue.Count != 0)
        {
            if (CheckNearestBlocks(_positionsQueue.Dequeue()) == false)
            {
                return false;
            }
        }
        
        return FinalCheck();
    }

    protected virtual bool FinalCheck() => true;

    public void SetPositionAtMatrix(int x, int y) => (_x, _y) = _matrix.ConvertXY(x, y);
    
    private bool CheckNearestBlocks((int, int) position)
    {
        var (x, y) = position;

        for (int i = -1; i < 2; i += 2)
        {
            if(_blockDict.ContainsKey((x + i, y)) == true) 
                continue;
            
            if (CheckBlockTypeAndColor(_matrix[x + i, y]))
            {
                if (_matrix[x + i, y].blockType == BlockType.Blocked)
                    continue;
                _blockDict.Add((x + i, y), _matrix[x + i, y]);
                _positionsQueue.Enqueue((x + i, y));
            }
            else
            {
                return false;
            }
        }
        
        for (int i = -1; i < 2; i += 2)
        {
            if(_blockDict.ContainsKey((x, y + i)) == true) 
                continue;
            
            if (CheckBlockTypeAndColor(_matrix[x, y + i]))
            {
                if (_matrix[x, y + i].blockType == BlockType.Blocked)
                    continue;
                _blockDict.Add((x, y + i), _matrix[x, y + i]);
                _positionsQueue.Enqueue((x, y + i));
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    protected abstract bool CheckBlockTypeAndColor(MatrixForConditions.BlockAtMatrix block);
    
    protected void Reset(int id)
    {
        if (id == ID)
        {
            _blockDict = new Dictionary<(int, int), MatrixForConditions.BlockAtMatrix>(_matrix.Length);
            _positionsQueue = new Queue<(int, int)>(_matrix.Length);
        }
    }
}
