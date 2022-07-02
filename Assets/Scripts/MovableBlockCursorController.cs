using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovableBlockCursorController : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 2f;
    
    private Rigidbody _rb;
    private float _xAxis;
    private float _yAxis;

    private float _xPosition;

    private float _scale;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _xPosition = transform.localPosition.x;

        _scale = transform.parent.lossyScale.z;
    }

    private void OnEnable()
    {
        LineController.singleton.onCancelGame += DestroyObj;
        LineController.singleton.onFinishGame += DestroyObj;
        LineController.singleton.onWrongFinishGame += DestroyObj;
    }

    private void OnDisable()
    {
        LineController.singleton.onCancelGame -= DestroyObj;
        LineController.singleton.onFinishGame -= DestroyObj;
        LineController.singleton.onWrongFinishGame -= DestroyObj;
    }

    private void Update()
    {
        GetAxis();
        Move();
    }

    private void FixedUpdate()
    {
        FreezePositionX();
    }

    private void GetAxis()
    {
        _xAxis = Mathf.Clamp(Input.GetAxis("Mouse X"), -2f, 2f);
        _yAxis = Mathf.Clamp(Input.GetAxis("Mouse Y"), -2f, 2f);
    }

    private void Move()
    {
        Vector3 direction = transform.up * _yAxis + transform.forward * _xAxis;
        _rb.velocity = direction * (_sensitivity * _scale);
    }

    private void FreezePositionX()
    {
        Vector3 pos = new Vector3(_xPosition, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = pos;
    }

    private void DestroyObj(int id)
    {
        Destroy(gameObject);
    }
}
