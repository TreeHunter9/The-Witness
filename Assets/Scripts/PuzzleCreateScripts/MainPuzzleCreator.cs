using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPuzzleCreator : MonoBehaviour
{
    [Header("Map and Step")]
    [SerializeField] private Texture2D _levelMapTexture;
    [Tooltip("Если не нужно, то ставь 0")]
    [SerializeField] private int _step;
    
    [Header("Materials")]
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField] private Material _pathMaterial;
    [SerializeField] private Material _lineMaterial;
    
    [Header("Puzzle Blocks")]
    [SerializeField] private PuzzleBlock _pathBlock;
    [SerializeField] private PuzzleBlock _pathBlockWithoutCorner;   //правый нижний угол закруглён
    [SerializeField] private PuzzleBlock _backgroundBlock;
    [SerializeField] private PuzzleBlock _startBlock;
    [SerializeField] private PuzzleBlock _endBlock;
    [SerializeField] private PuzzleBlock _deadEndBlock;
    
    private GameObject _parentObj;
    private Vector3 _panelSize;
    private Vector3 _blockSize;
    
    private readonly Color _pathColor = Color.black;
    private readonly Color _startColor = Color.green;
    private readonly Color _endColor = Color.red;
    private readonly Color _deadEndColor = Color.gray;

    private Vector2 _extraLineOnLevelMapTexture = new Vector2(0f, 0f);
    
    private GameObject[,] _matrixOfBlocks;
    private MatrixForConditions _matrixForConditions;

    private ConditionsOfFinish _conditionsOfFinish;

    private static int _panelCount = 0;
    private int _panelID;

    private void Awake()
    {
        _parentObj = new GameObject("LvlContainer");
        _parentObj.transform.position = transform.position;
        Vector3 panelMeshRendererSize = GetComponent<BoxCollider>().size;
        Vector3 blockMeshRendererSize = _backgroundBlock.blockGameObject.GetComponent<MeshRenderer>().bounds.size;
        Vector3 panelLossyScale = transform.lossyScale;
        _panelSize = new Vector3(panelMeshRendererSize.x * panelLossyScale.x,
            panelMeshRendererSize.y * panelLossyScale.y, panelMeshRendererSize.z * panelLossyScale.z);
        _blockSize = new Vector3(blockMeshRendererSize.x,
            blockMeshRendererSize.y, blockMeshRendererSize.z);

        _matrixOfBlocks = new GameObject[_levelMapTexture.width, _levelMapTexture.height];
        _levelMapTexture.GetPixels32();
        _panelCount++;
        _panelID = _panelCount;
        
        ChangeMaterial(gameObject, _backgroundMaterial);

        _conditionsOfFinish = GetComponent<ConditionsOfFinish>();
        if (_step != 0)
        {
            _matrixForConditions = new MatrixForConditions(_levelMapTexture, _step);
            _matrixForConditions.GenerateMatrix();
        }

        if (TryGetComponent(out ActionOnComplete levelComplete))
            levelComplete.ID = _panelID;

        CreateLevel();
    }
    
    private void CreateLevel()
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

        GenerateAdditionalLevelMap();
        
        float scale = FillAllPanel(yAxisSpace, zAxisSpace);
        _parentObj.transform.rotation = transform.rotation;
        _parentObj.transform.parent = transform;

        _parentObj.transform.localPosition -= new Vector3(0f,
            (yAxisSpace - _extraLineOnLevelMapTexture.y - _blockSize.z * scale) * 0.5f * _parentObj.transform.localScale.y,
            (zAxisSpace - _extraLineOnLevelMapTexture.x - _blockSize.z * scale) * 0.5f * _parentObj.transform.localScale.z);
    }

    private void GenerateAdditionalLevelMap()
    {
        AdditionalPuzzleCreator additionalPuzzleCreatorScript;
        if (additionalPuzzleCreatorScript = GetComponent<AdditionalPuzzleCreator>())
        {
            additionalPuzzleCreatorScript.GenerateAdditionalLevelMap(_parentObj, 
                _matrixOfBlocks, _matrixForConditions, _blockSize.z, _panelID);
        }
    }

    private float FillAllPanel(float yAxisSpace, float zAxisSpace)
    {
        float yScale = _panelSize.y / yAxisSpace * 0.9f;
        float zScale = _panelSize.z / zAxisSpace * 0.9f;
        float scale = zScale < yScale ? zScale : yScale;
        
        _parentObj.transform.localScale = new Vector3(1f, scale, scale);
        return scale;
    }

    private void GenerateTile(int x, int y, float zAxisSpace, float yAxisSpace)
    {
        Color pixelColor = _levelMapTexture.GetPixel(x, y);
        
        //The pixel is transparent
        if (pixelColor.a == 0f)
        {
            _matrixOfBlocks[x, y] = SpawnBlock(_backgroundBlock, zAxisSpace, yAxisSpace);
            ChangeMaterial(_matrixOfBlocks[x, y], _backgroundMaterial);
            return;
        }
        else if (pixelColor == _pathColor)
        {
            if (CheckPathBlockWithoutCorner(x, y, out var angle) == true)
            {          
                _matrixOfBlocks[x, y] = SpawnBlock(_pathBlockWithoutCorner, zAxisSpace, yAxisSpace);
                ChangeAngleOfBlock(_matrixOfBlocks[x, y], angle);
                EnableTurnBlock(x, y);
                ChangeMaterial(_matrixOfBlocks[x, y], _pathMaterial);
                ChangeLineRenderMaterial(_matrixOfBlocks[x, y]);
                return;
            }

            _matrixOfBlocks[x, y] = SpawnBlock(_pathBlock, zAxisSpace, yAxisSpace);
            if (CheckTurnBlock(x, y) == true)
            {
                EnableTurnBlock(x, y);
                ChangeLineRenderMaterial(_matrixOfBlocks[x, y]);
            }
        }
        else if (pixelColor == _startColor)
        {
            _matrixOfBlocks[x, y] = SpawnBlock(_startBlock, zAxisSpace, yAxisSpace, 0.00001f);
            _matrixOfBlocks[x, y].GetComponent<CreateFirstLine>().ID = _panelID;
            EnableBuildLineWayOnMatrix(x, y);
            ChangeLineRenderMaterial(_matrixOfBlocks[x, y]);
        }
        else if (pixelColor == _endColor)
        {
            _matrixOfBlocks[x, y] = SpawnBlock(_endBlock, zAxisSpace, yAxisSpace);
            TurnEndBlock(x, y);
            _matrixOfBlocks[x, y].GetComponent<EndBlockTrigger>().ConditionsOfFinish = _conditionsOfFinish;
            
            RemoveExtraLine(x, y);
        }
        else if (pixelColor == _deadEndColor)
        {
            _matrixOfBlocks[x, y] = SpawnBlock(_deadEndBlock, zAxisSpace, yAxisSpace);
            CheckDeadEndBlockRotation(x, y);
        }
        ChangeMaterial(_matrixOfBlocks[x, y], _pathMaterial);
    }

    private GameObject SpawnBlock(PuzzleBlock block, float zAxisSpace, float yAxisSpace, float additionalParameter = 0f)
    {
        Vector3 pos = new Vector3(_parentObj.transform.position.x + _panelSize.x / 2f + 0.0001f + additionalParameter, 
            _parentObj.transform.position.y + yAxisSpace,
            _parentObj.transform.position.z + zAxisSpace);
        return Instantiate(block.blockGameObject, pos, block.transform.rotation, _parentObj.transform);
    }

    private void ChangeMaterial(GameObject obj, Material material)
    {
        obj.GetComponent<Renderer>().material = material;
    }

    private void ChangeLineRenderMaterial(GameObject obj)
    {
        obj.GetComponent<LineRenderer>().material = _lineMaterial;
    }

    private void RemoveExtraLine(int x, int y)
    {
        for (int i = -1; i < 2; i += 2)
        {
            if (_levelMapTexture.GetPixel(x + i, y) == _pathColor)
                _extraLineOnLevelMapTexture.x = _blockSize.z;
        }
        
        for (int i = -1; i < 2; i += 2)
        {
            if (_levelMapTexture.GetPixel(x, y + i) == _pathColor)
                _extraLineOnLevelMapTexture.y = _blockSize.z;
        }
    }

    private bool CheckTurnBlock(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x - 1, y).a != 0f && 
            (_levelMapTexture.GetPixel(x, y - 1).a != 0f ||
        _levelMapTexture.GetPixel(x, y + 1).a != 0f))
        {
            return true;
        }
        return _levelMapTexture.GetPixel(x + 1, y).a != 0f && 
               (_levelMapTexture.GetPixel(x, y - 1).a != 0f ||
                _levelMapTexture.GetPixel(x, y + 1).a != 0f);
    }

    private bool CheckPathBlockWithoutCorner(int x, int y, out float angle)
    {
        if (_levelMapTexture.GetPixel(x + 1, y).a == 0f && _levelMapTexture.GetPixel(x, y - 1).a == 0f
        && _levelMapTexture.GetPixel(x - 1, y).a != 0f && _levelMapTexture.GetPixel(x, y + 1).a != 0f )
        {
            angle = 0f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y + 1).a == 0f && _levelMapTexture.GetPixel(x + 1, y).a == 0f
        && _levelMapTexture.GetPixel(x - 1, y).a != 0f && _levelMapTexture.GetPixel(x, y - 1).a != 0f)
        {

            angle = 90f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y + 1).a == 0f && _levelMapTexture.GetPixel(x - 1, y).a == 0f 
        && _levelMapTexture.GetPixel(x + 1, y).a != 0f && _levelMapTexture.GetPixel(x, y - 1).a != 0f)
        {
            angle = 180f;
            return true;
        }
        if (_levelMapTexture.GetPixel(x, y - 1).a == 0f && _levelMapTexture.GetPixel(x - 1, y).a == 0f
        && _levelMapTexture.GetPixel(x + 1, y).a != 0f && _levelMapTexture.GetPixel(x, y + 1).a != 0f)
        {
            angle = 270f;
            return true;
        }

        angle = 0f;
        return false;
    }

    private void ChangeAngleOfBlock(GameObject block, float angle)
    {
        block.transform.Rotate(Vector3.forward, angle, Space.Self);
    }

    private void CheckDeadEndBlockRotation(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x - 1, y) == _deadEndColor)
        {
            return;   
        }
        else if (_levelMapTexture.GetPixel(x, y - 1) == _deadEndColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 90f);
        }
        else if (_levelMapTexture.GetPixel(x + 1, y) == _deadEndColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 180f);
        }
        else if (_levelMapTexture.GetPixel(x, y + 1) == _deadEndColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 270f);
        }
    }

    private void TurnEndBlock(int x, int y)
    {
        if (_levelMapTexture.GetPixel(x, y - 1) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 0f);
        }
        else if (_levelMapTexture.GetPixel(x - 1, y) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 90f);
        }
        else if (_levelMapTexture.GetPixel(x, y + 1) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 180f);
        }
        else if (_levelMapTexture.GetPixel(x + 1, y) == _pathColor)
        {
            ChangeAngleOfBlock(_matrixOfBlocks[x, y], 270f);
        }
    }

    private void EnableTurnBlock(int x, int y)
    {
        BoxCollider[] mas = _matrixOfBlocks[x, y].GetComponents<BoxCollider>();
        foreach (BoxCollider col in mas)
        {
            if (col.isTrigger)
            {
                col.enabled = true;
            }
        }
        
        _matrixOfBlocks[x, y].GetComponent<LineRenderer>().enabled = true;
        AddNewLineOnTurn script = _matrixOfBlocks[x, y].GetComponent<AddNewLineOnTurn>();
        script.enabled = true;
        script.ID = _panelID;

        EnableBuildLineWayOnMatrix(x, y);
    }

    private void EnableBuildLineWayOnMatrix(int x, int y)
    {
        if (_step != 0)
        {
            _matrixOfBlocks[x, y].GetComponent<AddNewLineOnTurn>().BuildLineWayActive = true;
            _matrixOfBlocks[x, y].GetComponent<BuildLineWayOnMatrix>().Init(_panelID, _matrixForConditions, (x, y));
        }
    }
}
