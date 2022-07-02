using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    
    private float _verticalInput;
    private float _horizontalInput;

    private void Update()
    {
        GetMouseAxis();
        Rotation();
    }

    private void GetMouseAxis()
    {
        _horizontalInput = Input.GetAxis("Mouse X");
        _verticalInput = Input.GetAxis("Mouse Y") * -1f;
    }

    private void Rotation()
    {
        transform.Rotate(Vector3.up, _horizontalInput);
        _cameraTransform.Rotate(Vector3.right, _verticalInput);
    }
}
