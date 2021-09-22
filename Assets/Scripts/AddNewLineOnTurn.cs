using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AddNewLineOnTurn : MonoBehaviour
{
    private enum StateOfTrigger
    {
        Enter,
        Exit
    };
    private StateOfTrigger _stateOfTrigger = StateOfTrigger.Enter;
    
    private LineRenderer _lineRenderer;
    private LineRenderer _previousLineRenderer;
    private Vector3 _positionForLineRenderer;
    private Vector3 _enterDirection;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        LineController.singleton.onCancelGame += RemoveLine;    //В awake выдаёт ошибку))))
        
        _positionForLineRenderer = transform.position;
        _positionForLineRenderer.x += 0.00001f;
        _lineRenderer.SetPosition(0, _positionForLineRenderer);
        _lineRenderer.SetPosition(1, _positionForLineRenderer);
        _lineRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (_stateOfTrigger == StateOfTrigger.Enter)
        {
            _lineRenderer.enabled = true;
            _previousLineRenderer = LineController.singleton.currentLineRenderer;
            _previousLineRenderer.SetPosition(1, _positionForLineRenderer);
            LineController.singleton.currentLineRenderer = _lineRenderer;
            _stateOfTrigger = StateOfTrigger.Exit;
            
            _enterDirection = transform.position - LineController.singleton.movableBlockTransform.position;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (_stateOfTrigger == StateOfTrigger.Exit)
        {
            Vector3 exitDirection = LineController.singleton.movableBlockTransform.position - transform.position;
            if (Vector3.Dot(exitDirection.normalized, _enterDirection.normalized) < -0.7f)
            {
                _lineRenderer.enabled = false;
                _lineRenderer.SetPosition(1, _positionForLineRenderer);
                LineController.singleton.currentLineRenderer = _previousLineRenderer;
                _stateOfTrigger = StateOfTrigger.Enter;
            }
        }
    }

    private void RemoveLine()
    {
        _lineRenderer.SetPosition(1, _positionForLineRenderer);
        _lineRenderer.enabled = false;
    }
}
