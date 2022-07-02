using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CursorRestriction : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    
    private RectTransform _cursorRectTrans;
    private Vector2 _screenSize;
    private bool _gameIsStarted = false;

    public static CursorRestriction singleton;

    private void Awake()
    {
        singleton = this;
        
        _cursorRectTrans = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _screenSize = new Vector2(Screen.width / _canvas.scaleFactor, Screen.height / _canvas.scaleFactor);
    }

    private void Update()
    {
        if (_gameIsStarted == false)
        {
            CursorMovement();
        }
    }

    private void CursorMovement()
    {
        Vector3 pos = _cursorRectTrans.localPosition +
                      new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 14f;
        if (pos.x < _screenSize.x / 2f * -1)
            pos.x = _screenSize.x / 2f * -1;
        if (pos.x > _screenSize.x / 2f)
            pos.x = _screenSize.x / 2f;
        if (pos.y < _screenSize.y / 2f * -1) 
            pos.y = _screenSize.y / 2f * -1;
        if (pos.y > _screenSize.y / 2f) 
            pos.y = _screenSize.y / 2f;
        _cursorRectTrans.localPosition = pos;
    }
}
