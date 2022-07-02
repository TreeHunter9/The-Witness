using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MyPlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    
    private Rigidbody _rb;
    private Vector3 _movementDirection;
    private float _verticalInput;
    private float _horizontalInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetMoveAxis();
        Movement();
    }

    private void GetMoveAxis()
    {
        _verticalInput = Input.GetAxis("Vertical");
        _horizontalInput = Input.GetAxis("Horizontal");
    }

    private void Movement()
    {
        _movementDirection = transform.forward * _verticalInput + transform.right * _horizontalInput;
        _rb.velocity = _movementDirection * _movementSpeed;
    }

    public void StopVelocity()
    {
        _rb.velocity = Vector3.zero;
    }
}
