using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnterGameMode : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panelUI;
    [SerializeField] private RectTransform _cursorRectTransform;
    [SerializeField] private AnimationCurve _appearCurve;
    [SerializeField] private AnimationCurve _disappearCurve;
    [SerializeField] private TextMeshProUGUI _fps;
    
    private AnimationCurve _currentAnimCurve;
    private Vector2 _screenSize;
    private bool _enumeratorIsActive = false;
    private bool _UIVisible = false;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Start()
    {
        _currentAnimCurve = _appearCurve;
        _screenSize = _cursorRectTransform.sizeDelta;
    }

    private void Update()
    {
        UIAppear();
        _fps.text = (1.0f / Time.deltaTime).ToString();
    }

    private void UIAppear()
    {
        if (Input.GetMouseButtonDown(0) && _UIVisible == false)
        {
            _cursorRectTransform.localPosition = Vector3.zero;
            _currentAnimCurve = _appearCurve;
            if (_enumeratorIsActive == false)
            {
                StartCoroutine(ShowUI());
            }
            _UIVisible = true;
        }
        else if (Input.GetMouseButtonDown(1) && _UIVisible == true)
        {
            _currentAnimCurve = _disappearCurve;
            if (_enumeratorIsActive == false)
            {
                StartCoroutine(ShowUI());
            }
            _UIVisible = false;
        }
    }

    private IEnumerator ShowUI()
    {
        _enumeratorIsActive = true;
        for (float time = 0.0f; time <= 1.1f; time += 0.1f)
        {
            _panelUI.alpha = _currentAnimCurve.Evaluate(time);
            yield return new WaitForSeconds(0.05f);
        }
        _enumeratorIsActive = false;
    }
}
