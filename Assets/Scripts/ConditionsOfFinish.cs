using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConditionsOfFinish : MonoBehaviour
{
    private List<Condition> _listOfConditionsGO = new List<Condition>();

    public void ListAdd(Condition value) => _listOfConditionsGO.Add(value);
    
    public bool CheckConditions()
    {
        bool isGood = true;
        foreach (var element in _listOfConditionsGO)
        {
            if (element.CheckCondition() == true)
            {
                continue;
            }
            else
            {
                element.Mistake();
                isGood = false;
            }
        }

        return isGood;
    }
}
