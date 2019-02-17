using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{

    private IImput[] imputs;
    private ShaderImputHandlerBase[] shaderImputHandlers;

    public void PopulateUniforms(Material mat)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        // load input & handler as components
        shaderImputHandlers = GetComponents<ShaderImputHandlerBase>();
        Debug.Log("Get ["+ shaderImputHandlers.Length + "] shader Handlers");
        string logStr = "Handler names : \n";
        foreach (ShaderImputHandlerBase sh in shaderImputHandlers)
        {
            logStr += "\t- "+sh.HandlerName + "\n";
        }
        Debug.Log(logStr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
