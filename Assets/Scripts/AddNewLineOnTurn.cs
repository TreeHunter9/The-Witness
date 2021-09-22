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

    private BoxCollider _lineCollider;

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
            EnableCollider();
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (_stateOfTrigger == StateOfTrigger.Exit)
        {
            Vector3 exitDirection = LineController.singleton.movableBlockTransform.position - transform.position;
            if (Vector3.Dot(exitDirection.normalized, _enterDirection.normalized) < -0.7f)  //проверка чтобы убрать линию по которой вошли на повороте
            {
                _lineRenderer.enabled = false;
                DisableCollider();
                _lineRenderer.SetPosition(1, _positionForLineRenderer);
                LineController.singleton.currentLineRenderer = _previousLineRenderer;
                _stateOfTrigger = StateOfTrigger.Enter;
            }
        }
    }

    private void EnableCollider()
    {
        _lineCollider = _previousLineRenderer.gameObject.GetComponent<BoxCollider>();
        Vector3 pos = transform.localPosition - _lineCollider.transform.localPosition;
        Vector3 center = new Vector3(pos.z / _lineCollider.transform.lossyScale.x / 2f, 
            pos.y / _lineCollider.transform.lossyScale.y / 2f, pos.x);       
        _lineCollider.center = center - center.normalized / 1.5f;     //сдвигаем центр на пол квадрата чтобы на повороте был коллайдер
        Vector3 size = new Vector3(Mathf.Clamp(Mathf.Abs(center.x * 2f) - 0.5f, 1f, 1000f), 
            Mathf.Clamp(Mathf.Abs(center.y * 2f) - 0.5f, 1f, 1000f), _lineCollider.size.z);
        _lineCollider.size = size;
        _lineCollider.enabled = true;
        _lineCollider.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void DisableCollider()
    {
        _lineCollider.gameObject.layer = LayerMask.NameToLayer("Turn");
        _lineCollider.enabled = false;
    }

    private void RemoveLine()
    {
        _lineRenderer.SetPosition(1, _positionForLineRenderer);
        _lineRenderer.enabled = false;
    }
}
