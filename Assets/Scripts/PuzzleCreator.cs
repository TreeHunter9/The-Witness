using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCreator : MonoBehaviour
{
    [SerializeField] private Texture2D _levelMapTexture;
    [SerializeField] private PuzzleBlock _pathBlock;
    [SerializeField] private PuzzleBlock _backgroundBlock;
    [SerializeField] private PuzzleBlock _startBlock;
    [SerializeField] private PuzzleBlock _endBlock;

    private GameObject _parentObj;
    private Vector3 _panelSize;
    private Vector3 _blockSize;
    private Color _pathColor = Color.black;
    private Color _startColor = Color.green;
    private Color _endColor = Color.red;

    private GameObject _tempBlock;
    private GameObject[,] _matrixOfBlocks;

    private void Awake()
    {
        _parentObj = new GameObject("LvlContainer");
        _parentObj.transform.position = transform.position;
        Vector3 panelMeshRendererSize = GetComponent<BoxCollider>().size;
        Vector3 blockMeshRendererSize = _backgroundBlock.blockGameObject.GetComponent<MeshRenderer>().bounds.size;
        var panelLossyScale = transform.lossyScale;
        var blockLossyScale = _backgroundBlock.transform.lossyScale;
        _panelSize = new Vector3(panelMeshRendererSize.x * panelLossyScale.x,
            panelMeshRendererSize.y * panelLossyScale.y, panelMeshRendererSize.z * panelLossyScale.z);
        _blockSize = new Vector3(blockMeshRendererSize.x * blockLossyScale.x,
            blockMeshRendererSize.y * blockLossyScale.y, blockMeshRendererSize.z * blockLossyScale.z);

        _matrixOfBlocks = new GameObject[_levelMapTexture.width, _levelMapTexture.height];
    }

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        float zAxisSpace = 0;
        float yAxisSpace = 0;
        for (int x = 0; x < _levelMapTexture.width; x++)
        {
            yAxisSpace = 0;
            for (int y = 0; y < _levelMapTexture.height; y++)
            {
                GenerateTile(x, y, zAxisSpace, yAxisSpace);
                yAxisSpace += _blockSize.z;
            }
            zAxisSpace += _blockSize.z;
        }
        _parentObj.transform.position = new Vector3(_parentObj.transform.position.x,
            _parentObj.transform.position.y + (_panelSize.y - yAxisSpace) / 2f,
            _parentObj.transform.position.z + (_panelSize.z - zAxisSpace) / 2f);
    }

    private void GenerateTile(int x, int y, float zAxisSpace, float yAxisSpace)
    {
        Color pixleColor = _levelMapTexture.GetPixel(x, y);
        
        //The pixel is transparent
        if (pixleColor.a == 0)
        {
            _matrixOfBlocks[x,y] = SpawnBlock(_backgroundBlock, zAxisSpace, yAxisSpace);
        }
        else if (pixleColor == _pathColor)
        {
            _matrixOfBlocks[x,y] = SpawnBlock(_pathBlock, zAxisSpace, yAxisSpace);
            if (CheckTurnBlock(x, y))
            {
                EnableTurnBlock(x, y);
            }
        }
        else if (pixleColor == _startColor)
        {
            _matrixOfBlocks[x,y] = SpawnBlock(_startBlock, zAxisSpace, yAxisSpace, 0.00001f);
        }
        else if (pixleColor == _endColor)
        {
            _matrixOfBlocks[x,y] = SpawnBlock(_endBlock, zAxisSpace, yAxisSpace);
            AddTurnBlockAtEndLevel(x, y);
        }
    }

    private GameObject SpawnBlock(PuzzleBlock block, float zAxisSpace, float yAxisSpace, float additionalParameter = 0f)
    {
        Vector3 pos = new Vector3(transform.position.x + _panelSize.x / 2f + 0.00001f + additionalParameter, 
            transform.position.y - _panelSize.y / 2f + yAxisSpace + _blockSize.z / 2f,
            transform.position.z - _panelSize.z / 2f + zAxisSpace + _blockSize.z / 2f);
        return Instantiate(block.blockGameObject, pos, block.blockGameObject.transform.rotation, _parentObj.transform);
    }

    private bool CheckTurnBlock(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x - 1, y) == _pathColor && 
            (_levelMapTexture.GetPixel(x, y - 1) == _pathColor ||
        _levelMapTexture.GetPixel(x, y + 1) == _pathColor))
        {
            return true;
        }
        return _levelMapTexture.GetPixel(x + 1, y) == _pathColor &&
               (_levelMapTexture.GetPixel(x, y - 1) == _pathColor ||
                _levelMapTexture.GetPixel(x, y + 1) == _pathColor);
    }

    private void AddTurnBlockAtEndLevel(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x - 1, y) == _pathColor)
        {
            EnableTurnBlock(x - 1, y);
        }
        else if (_levelMapTexture.GetPixel(x + 1, y) == _pathColor)
        {
            EnableTurnBlock(x + 1, y);
        }
        else if (_levelMapTexture.GetPixel(x, y - 1) == _pathColor)
        {
            EnableTurnBlock(x, y - 1);
        }
        else if (_levelMapTexture.GetPixel(x, y + 1) == _pathColor)
        {
            EnableTurnBlock(x, y + 1);
        }
    }

    private void EnableTurnBlock(int x, int y)
    {
        _matrixOfBlocks[x,y].GetComponent<SphereCollider>().enabled = true;
        _matrixOfBlocks[x,y].GetComponent<LineRenderer>().enabled = true;
    }

}
