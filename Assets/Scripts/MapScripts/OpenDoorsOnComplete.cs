using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenDoorsOnComplete : ActionOnComplete
{
    [SerializeField] private Transform _rDoorTransform;
    [SerializeField] private Transform _lDoorTransform;

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
            RotateRDoorAsync();
            RotateLDoorAsync();
            gameObject.SetActive(false);
        }
    }

    private async Task RotateRDoorAsync()
    {
        while (_rDoorTransform.transform.rotation.eulerAngles.y < 90)
        {
            _rDoorTransform.transform.Rotate(Vector3.forward, -0.3f);
            await Task.Yield();
        }
    }
    
    private async Task RotateLDoorAsync()
    {
        while (_lDoorTransform.transform.rotation.eulerAngles.y > 90)
        {
            _lDoorTransform.transform.Rotate(Vector3.forward, 0.3f);
            await Task.Yield();
        }
    }
}
