using System;
using UnityEngine;

public abstract class InputBase : MonoBehaviour
{
    protected string inputName = "to_fill";

    public string InputName { get => inputName; }

    public virtual void StartInput()
    {

    }

    public virtual void UpdateInput()
    {

    }

    public virtual void StopInput()
    {
    }
}