using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGameMode : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panelUI;
    [SerializeField] private AnimationCurve _appearCurve;
    [SerializeField] private AnimationCurve _disappearCurve;
    
    private AnimationCurve _currentAnimCurve;
    private bool _enumeratorIsActive = false;
    private bool _UIVisible = false;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Start()
    {
        _currentAnimCurve = _appearCurve;
    }

    private void Update()
    {
        UIAppear();
    }

    private void UIAppear()
    {
        if (Input.GetMouseButtonDown(0) && _UIVisible == false)
        {
            _currentAnimCurve = _appearCurve;
            StartCoroutine(ShowUI());
            _UIVisible = true;
        }
        else if (Input.GetMouseButtonDown(1) && _UIVisible == true)
        {
            _currentAnimCurve = _disappearCurve;
            StartCoroutine(ShowUI());
            _UIVisible = false;
        }
    }

    private IEnumerator ShowUI()
    {
        if (_enumeratorIsActive == true)
            yield break;
        
        _enumeratorIsActive = true;
        for (float time = 0.0f; time <= 1.1f; time += 0.1f)
        {
            _panelUI.alpha = _currentAnimCurve.Evaluate(time);
            yield return new WaitForSeconds(0.05f);
        }
        _enumeratorIsActive = false;
    }
}
