using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBlockTrigger : MonoBehaviour
{
    private bool _atEnd = false;

    public event Action<Vector3> onFinishLevel;
    
    public ConditionsOfFinish ConditionsOfFinish { get; set; }
    
    public int ID { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovableBlock") && _atEnd != true)
        {
            _atEnd = true;
            onFinishLevel += LineController.singleton.FinishLevel;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovableBlock") && _atEnd != false)
        {
            _atEnd = false;
            onFinishLevel -= LineController.singleton.FinishLevel;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _atEnd == true)
        {
            if (ConditionsOfFinish != null)
            {
                if (ConditionsOfFinish.CheckConditions() == false)
                {
                    LineController.singleton.WrongFinish();
                    _atEnd = false;
                    return;
                }
            }

            onFinishLevel?.Invoke(transform.position + transform.forward * 0.0002f);

            onFinishLevel -= LineController.singleton.FinishLevel;
            
            this.enabled = false;
        }
    }
}
