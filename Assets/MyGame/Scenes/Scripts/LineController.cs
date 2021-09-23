using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static LineController singleton { get; private set; }
    
    public event Action onCancelGame;
    
    public event Action onStartGame;
    
    public event Action onFinishGame;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _sensitivity = 2f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _cursorObj;
    [SerializeField] private PuzzleBlock _movableBlockCursor;

    private GameObject _parentObj;
    private GameObject _movableBlockObj;
    private Rigidbody _rbMovableBlock;
    private RectTransform _cursorRectTrans;
    private bool _isStarted = false;

    private Vector3 _direction;


    public Transform movableBlockTransform { get; private set; }
    public LineRenderer currentLineRenderer { get; set; }


    private void Awake()
    {
        singleton = this;
        
        _parentObj = new GameObject("LineContainer");
        _cursorRectTrans = _cursorObj.GetComponent<RectTransform>();
    }

    private void Update()
    {
        MousClicksOnLevel();
    }

    private void FixedUpdate()
    {
        if (_isStarted == true)
        {
            CursorMovementOnPlaying();
            DrawLineRenderer();
        }
    }

    private void CursorMovementOnPlaying()
    {
        _direction = new Vector3(0f, Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        _rbMovableBlock.velocity = _direction * _sensitivity;
        _cursorRectTrans.position = _movableBlockCursor.transform.position;
    }

    private void DrawLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            Vector3 pos = _movableBlockObj.transform.position;
            pos.x -= 0.00001f;
            currentLineRenderer.SetPosition(1, pos);
        }
    }

    private void MousClicksOnLevel()
    {
        if (Input.GetMouseButtonDown(0) && _isStarted == false)
        {
            StartLevel();
        }
        else if (Input.GetMouseButtonDown(1) && _isStarted == true)
        {
            CancelLevel();
        }
        else if (Input.GetMouseButtonDown(0) && _isStarted == true)
        {
            FinishLevel();
        }
    }

    private void StartLevel()
    {
        Ray ray = _mainCamera.ScreenPointToRay(_cursorRectTrans.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f, _layerMask))
        {
            if (hit.collider != null)
            {
                Vector3 pos = hit.collider.transform.position;
                pos.x += 0.0001f;
                _movableBlockObj = Instantiate(_movableBlockCursor.blockGameObject, pos,
                    _movableBlockCursor.transform.rotation);
                _rbMovableBlock = _movableBlockObj.GetComponent<Rigidbody>();
                movableBlockTransform = _movableBlockObj.transform;
                onStartGame?.Invoke();
                _isStarted = true;
            }
        }
    }

    private void CancelLevel()
    {
        onCancelGame?.Invoke();
        _isStarted = false;
    }

    private void FinishLevel()
    {
        onFinishGame?.Invoke();
        _isStarted = false;
    }
}
