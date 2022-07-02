using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenSmallDoorOnComplete : ActionOnComplete
{
    [SerializeField] private Transform _doorTransform;
    
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
        {
            OpenDoorAsync();
            gameObject.SetActive(false);
        }
    }

    private async Task OpenDoorAsync()
    {
        while (_doorTransform.transform.rotation.eulerAngles.y < 275f)
        {
            _doorTransform.transform.Rotate(Vector3.forward, 0.3f);
            await Task.Yield();
        }
    }
}
