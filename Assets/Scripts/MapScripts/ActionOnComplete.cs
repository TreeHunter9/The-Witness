using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionOnComplete : MonoBehaviour
{
    public int ID { get; set; }
    protected abstract void Action(int id);
}
