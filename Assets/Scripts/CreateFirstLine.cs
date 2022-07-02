using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFirstLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3 _positionForLineRenderer;
    private AddNewLineOnTurn _addNewLineOnTurnScript;
    private SphereCollider _sphereCollider;
    private BoxCollider _boxCollider;

    public int ID { get; set; }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
        _addNewLineOnTurnScript = GetComponent<AddNewLineOnTurn>();
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
        LineController.singleton.onStartGame += EnableTurnScript;
        LineController.singleton.onCancelGame += RemoveLine;
        LineController.singleton.onCancelGame += DisableTurnScript;
        LineController.singleton.onWrongFinishGame += RemoveLine;
        LineController.singleton.onWrongFinishGame += DisableTurnScript;
        LineController.singleton.onFinishGame += FinishGame;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= EnableTurnScript;
        LineController.singleton.onCancelGame -= RemoveLine;
        LineController.singleton.onCancelGame -= DisableTurnScript;
        LineController.singleton.onWrongFinishGame -= RemoveLine;
        LineController.singleton.onWrongFinishGame -= DisableTurnScript;
        LineController.singleton.onFinishGame -= FinishGame;
    }

    public void CreateLine()
    {
        _lineRenderer.enabled = true;
        LineController.singleton.CurrentLineRenderer = _lineRenderer;
        
        _sphereCollider.enabled = false;
    }

    private void RemoveLine(int id)
    {
        if (ID == id)
        {
            _addNewLineOnTurnScript.enabled = true;
            _lineRenderer.enabled = false;
            _lineRenderer.SetPosition(1, _positionForLineRenderer);
            _boxCollider.enabled = false;
        }
    }

    private void FinishGame(int id)
    {
        if (ID == id)
        {
            gameObject.tag = "Untagged";
            
            this.enabled = false;
        }
    }

    private void DisableTurnScript(int id)
    {
        if (ID == id)
        {
            _addNewLineOnTurnScript.enabled = false;
            _sphereCollider.enabled = true;
        }
    }
    
    private void EnableTurnScript(int id)
    {
        if (ID == id && _lineRenderer.enabled == false)
        {
            _addNewLineOnTurnScript.enabled = true;
        }
    }
}
