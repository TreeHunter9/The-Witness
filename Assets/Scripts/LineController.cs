using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _cursorObj;
    [SerializeField] private PuzzleBlock _movableBlockCursor;
    
    private GameObject _parentObj;
    private RectTransform _cursorRectTrans;
    
    private Vector3 _distanceBetweenCursorAndStartPoint;

    private Image _cursorImage;

    private int _levelID;

    public bool IsStarted { get; private set; }
    public Transform MovableBlockTransform { get; private set; }
    public LineRenderer CurrentLineRenderer { get; set; }
    
    public static LineController singleton { get; private set; }

    public event Action<int> onStartGame;
    
    public event Action<int> onCancelGame;
    
    public event Action<int> onFinishGame;

    public event Action<int> onWrongFinishGame;


    private void Awake()
    {
        singleton = this;
        
        _parentObj = new GameObject("LineContainer");
        IsStarted = false;
    }

    private void Start()
    {
        _cursorRectTrans = _cursorObj.GetComponent<RectTransform>();
        _cursorImage = _cursorObj.GetComponent<Image>();
    }

    private void Update()
    {
        MousClicksOnLevel();
    }

    private void FixedUpdate()
    {
        if (IsStarted == true)
        {
            DrawLineRenderer();
        }
    }

    private void DrawLineRenderer()
    {
        if (CurrentLineRenderer != null)
        {
            Vector3 pos = MovableBlockTransform.position;
            pos -= _distanceBetweenCursorAndStartPoint;
            CurrentLineRenderer.SetPosition(1, pos);
        }
    }

    private void MousClicksOnLevel()
    {
        if (Input.GetMouseButtonDown(0) && IsStarted == false)
        {
            StartLevel();
        }
        else if (Input.GetMouseButtonDown(1) && IsStarted == true)
        {
            CancelLevel();
        }
    }

    private void StartLevel()
    {
        Ray ray = _mainCamera.ScreenPointToRay(_cursorRectTrans.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f, _layerMask))
        {
            if (hit.collider.CompareTag("Start"))
            {
                Vector3 pos = hit.collider.transform.position;
                _parentObj = hit.collider.transform.parent.gameObject;
                pos -= hit.collider.transform.forward * 0.0005f;
                MovableBlockTransform = Instantiate(_movableBlockCursor.blockGameObject, pos,
                    _parentObj.transform.rotation, _parentObj.transform).transform;
                
                CreateFirstLine createFirstLineScript = hit.collider.gameObject.GetComponent<CreateFirstLine>();
                _levelID = createFirstLineScript.ID;
                createFirstLineScript.CreateLine();
                _distanceBetweenCursorAndStartPoint = MovableBlockTransform.position - pos;

                onStartGame?.Invoke(_levelID);
                IsStarted = true;
                CursorActive(false);
            }
        }
    }

    private void CancelLevel()
    {
        onCancelGame?.Invoke(_levelID);
        
        IsStarted = false;
        
        CursorActive(true);
    }

    public void FinishLevel(Vector3 pos)
    {
        CurrentLineRenderer.SetPosition(1, pos);
        
        IsStarted = false;
        
        onFinishGame?.Invoke(_levelID);

        CursorActive(true);
    }

    public void WrongFinish()
    {
        onWrongFinishGame?.Invoke(_levelID);
        
        IsStarted = false;
        
        CursorActive(true);
    }


    private void CursorActive(bool active)
    {
        _cursorImage.enabled = active;
        _cursorRectTrans.position = _mainCamera.WorldToScreenPoint(MovableBlockTransform.position);

        EnterGameMode.singleton.enabled = active;
        CursorRestriction.singleton.enabled = active;
    }
}
