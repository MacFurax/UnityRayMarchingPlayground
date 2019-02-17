using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShaderImputHandlerBase : MonoBehaviour
{
    protected string handlerName = "<change me>";
    public Shader _shader;

    public string HandlerName { get => handlerName; }

    protected virtual void PopulateUniforms( ref Material mat)
    {
    }

    protected virtual void StartHandler()
    {
    }

    protected virtual void UpdateHandler()
    {
    }

    protected virtual void StopHandler()
    {
    }
}
