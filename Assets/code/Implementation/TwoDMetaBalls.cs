﻿using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDMetaBalls : ShaderImputHandlerBase
{
    private OSCSource oscSource;

    public TwoDMetaBalls()
    {
        handlerName = "2D Metaballs";
    }

    public override void PopulateUniforms(ref Material mat)
    {
        
    }

    public override void StartHandler()
    {
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/2DMetaballs/*", MessageReceived);

    }

    public override void StopHandler()
    {
        
    }

    public override void UpdateHandler()
    {
        
    }

    private void MessageReceived(OSCMessage message)
    {

    }
}
