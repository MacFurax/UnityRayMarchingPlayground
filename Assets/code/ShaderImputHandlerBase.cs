using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShaderImputHandlerBase : MonoBehaviour
{
    protected string handlerName = "<change me>";
    public Shader _shader;

    public string HandlerName { get => handlerName; }

    public virtual void PopulateUniforms( ref Material mat)
    {

    }

    public virtual void Init()
    {

    }

    public virtual void UpdateHandler()
    {

    }

    public virtual void Close()
    {

    }
}
