using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPointTrigger : Condition
{
    private Vector3 _firstEnterDirection;
    private bool _isPassed = false;

    private void OnEnable()
    {
        LineController.singleton.onStartGame += base.StopPingPongColor;
        LineController.singleton.onStartGame += Refresh;
    }

    private void OnDisable()
    {
        LineController.singleton.onStartGame -= base.StopPingPongColor;
        LineController.singleton.onStartGame -= Refresh;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (_isPassed == false)
        {
            _firstEnterDirection = coll.transform.position - transform.position;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        Vector3 exitDirection = coll.transform.position - transform.position;
        if (Vector3.Dot(exitDirection.normalized, _firstEnterDirection.normalized) >= 0.8f)
        {
            _isPassed = false;
        }
        else
        {
            _isPassed = true;
        }
    }

    public override bool CheckCondition() => _isPassed;

    private void Refresh(int id)
    {
        if (id == ID)
            _isPassed = false;
    }
}
