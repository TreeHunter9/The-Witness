using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNextLevelOnComplete : ActionOnComplete
{
    [SerializeField] private GameObject _nextLevel;

    private void OnEnable()
    {
        LineController.singleton.onFinishGame += Action;
    }

    private void OnDisable()
    {
        LineController.singleton.onFinishGame -= Action;
    }
    
    protected override void Action(int id)
    {
        if (id == base.ID)
            _nextLevel.SetActive(true);
    }
}
