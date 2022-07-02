using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewLineOnTurn : MonoBehaviour
{
    private enum StateOfTrigger
    {
        Waiting,
        Active
    };
    private StateOfTrigger _stateOfTrigger = StateOfTrigger.Waiting;
    
    private LineRenderer _lineRenderer;
    private LineRenderer _previousLineRenderer;
    private Vector3 _positionForLineRenderer;
    private Vector3 _firstEnterDirection;
    private Vector3 _firstEnterPosition;
    private bool _isComeBack = false;

    private BoxCollider _boxCollider;
    private BoxCollider _previousLineCollider;

    private BuildLineWayOnMatrix _buildLineWayOnMatrixScript;

    public int ID { get; set; }
    public bool BuildLineWayActive { get; set; } = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _boxCollider = GetComponent<BoxCollider>();

        _buildLineWayOnMatrixScript = GetComponent<BuildLineWayOnMatrix>();
    }

    private void Start()
    {
        _positionForLineRenderer = transform.position;
        _positionForLineRenderer -= transform.forward * 0.0003f;
        _lineRenderer.SetPosition(0, _positionForLineRenderer);
        _lineRenderer.SetPosition(1, _positionForLineRenderer);
        _lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        LineController.singleton.onCancelGame += RemoveLine;
        LineController.singleton.onWrongFinishGame += RemoveLine;
    }

    private void OnDisable()
    {
        LineController.singleton.onCancelGame -= RemoveLine;
        LineController.singleton.onWrongFinishGame -= RemoveLine;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (_stateOfTrigger == StateOfTrigger.Waiting)
        {
            _stateOfTrigger = StateOfTrigger.Active;
            
            _firstEnterPosition = LineController.singleton.MovableBlockTransform.position;
            _firstEnterDirection = transform.position - _firstEnterPosition;
            
            _previousLineRenderer = LineController.singleton.CurrentLineRenderer;

            if (BuildLineWayActive == true)
                _buildLineWayOnMatrixScript.BlockTheWay(_previousLineRenderer);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 direction = LineController.singleton.MovableBlockTransform.position - transform.position;
        float dot = Vector3.Dot(direction.normalized, _firstEnterDirection.normalized);

        if (dot >= -0.3f && _lineRenderer.enabled == false)
        {
            _isComeBack = false;
            
            _lineRenderer.enabled = true;
            _previousLineRenderer.SetPosition(1, _positionForLineRenderer);
            LineController.singleton.CurrentLineRenderer = _lineRenderer;
            EnableCollider();
        }
        else if (dot <= -0.7f && _lineRenderer.enabled == true)
        {
            _isComeBack = true;
            
            _lineRenderer.enabled = false;
            LineController.singleton.CurrentLineRenderer = _previousLineRenderer;
            _lineRenderer.SetPosition(1, _positionForLineRenderer);
            DisableCollider();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isComeBack == true)
        {
            _stateOfTrigger = StateOfTrigger.Waiting;
            
            if (BuildLineWayActive == true)
                _buildLineWayOnMatrixScript.Reset();
        }
    }

    private void EnableCollider()
    {
        _previousLineCollider = _previousLineRenderer.gameObject.GetComponent<BoxCollider>();
        _previousLineCollider.enabled = true;
        _previousLineCollider.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void DisableCollider()
    {
        if (_previousLineCollider != null)
        {
            _previousLineCollider.gameObject.layer = LayerMask.NameToLayer("Turn");
            _previousLineCollider.enabled = false;
        }
    }

    private void RemoveLine(int id)
    {
        if (ID == id)
        {
            _lineRenderer.SetPosition(1, _positionForLineRenderer);
            _lineRenderer.enabled = false;
            _stateOfTrigger = StateOfTrigger.Waiting;

            _boxCollider.enabled = false;
            DisableCollider();
        }
    }
}
