using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private Vector2 GetInputLookRotation()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
    }

    private void Rotation()
    {
        
    }
}
