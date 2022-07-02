using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatrixForConditions.BlockAtMatrix;

public class MatrixForConditions
{
    public struct BlockAtMatrix
    {
        public enum BlockType : ushort
        {
            Empty,
            Blocked,
            ColoredSquare,
            ColoredStar
        }

        public BlockType blockType;
        public Color blockColor;
    }

    private readonly Texture2D _level;
    private readonly int _step;
    
    private BlockAtMatrix[,] _matrixOfBlockTypes;
    private int _matrixSizeX;
    private int _matrixSizeY;
    
    private int _x;
    private int _y;
    
    private int _startPointX;
    private int _startPointY;

    public MatrixForConditions(Texture2D level, int step)
    {
        _level = level;
        _step = step;
    }

    public BlockAtMatrix this[int i, int j] => _matrixOfBlockTypes[i, j];

    public void SetBlockType(int i, int j, BlockType type)
    {
        _matrixOfBlockTypes[i, j].blockType = type;
    }

    public int Length => _matrixOfBlockTypes.Length;

    public void GenerateMatrix()
    {
        _startPointX = FindStartPointX();
        _startPointY = FindStartPointY();

        _matrixSizeX = Mathf.CeilToInt((_level.width - _startPointX - 1) / ((float) _step / 2)) + 2;
        _matrixSizeY = Mathf.CeilToInt((_level.height - _startPointY - 1) / ((float) _step / 2)) + 2;
        
        _matrixOfBlockTypes = new BlockAtMatrix[_matrixSizeX, _matrixSizeY];
        AddBorders();
    }

    private int FindStartPointX()
    {
        for (int x = 0; x < _level.width; x++)
        {
            for (int y = 0; y < _level.height; y++)
            {
                if (_level.GetPixel(x, y).a != 0f && _level.GetPixel(x, y) != Color.red)
                {
                    return x;
                }
            }
        }

        return -1;
    }
    
    private int FindStartPointY()
    {
        for (int y = 0; y < _level.height; y++)
        {
            for (int x = 0; x < _level.width; x++)
            {
                if (_level.GetPixel(x, y).a != 0f && _level.GetPixel(x, y) != Color.red)
                {
                    return y;
                }
            }
        }
        
        return -1;
    }

    public void ConvertToMatrix(int x, int y, BlockType type, Color color)
    {
        (_x, _y) = ConvertXY(x, y);
        _matrixOfBlockTypes[_x, _y].blockType = type;
        _matrixOfBlockTypes[_x, _y].blockColor = color;
    }

    public (int, int) ConvertXY(int x, int y)
    {
        return (Mathf.FloorToInt((x - _startPointX) / (float) _step) + 2 + x / _step,
            Mathf.FloorToInt((y - _startPointY) / (float) _step) + 2 + y / _step); // + 2 тк мы пропускаем 0,0 элемент и + ещё граница + учитываем сколько мы прошли делимых дорог
    }

    public void AddBorders()
    {
        for (int i = 0; i < _matrixSizeX; i += _matrixSizeX - 1)
        {
            for (int j = 0; j < _matrixSizeY; j++)
            {
                _matrixOfBlockTypes[i, j].blockType = BlockType.Blocked;
            }
        }
        
        for (int i = 0; i < _matrixSizeY; i += _matrixSizeY - 1)
        {
            for (int j = 0; j < _matrixSizeX; j++)
            {
                _matrixOfBlockTypes[j, i].blockType = BlockType.Blocked;
            }
        }
    }
}
