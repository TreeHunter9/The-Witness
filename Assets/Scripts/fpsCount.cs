using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fpsCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fps;

    private void Start()
    {
        StartCoroutine(CheckFPS());
    }

    private IEnumerator CheckFPS()
    {
        while (true)
        {
            _fps.text = (1.0f / Time.deltaTime).ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
