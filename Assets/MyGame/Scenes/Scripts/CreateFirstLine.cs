using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFirstLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3 _positionForLineRenderer;

    private void Awake()
    {
        LineController.singleton.onStartGame += CreateLine;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _positionForLineRenderer = transform.position;
        _positionForLineRenderer.x += 0.00001f;
        _lineRenderer.SetPosition(0, _positionForLineRenderer);
        _lineRenderer.SetPosition(1, _positionForLineRenderer);
        _lineRenderer.enabled = false;
    }

    private void CreateLine()
    {
        _lineRenderer.enabled = true;
        LineController.singleton.currentLineRenderer = _lineRenderer;
    }
}
