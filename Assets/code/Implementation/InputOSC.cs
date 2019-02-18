using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOSC : InputBase
{
    OSCSource oscSource;

    public InputOSC()
    {
        inputName = "OSC input";
    }

    public override void StartInput()
    {
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/input/*", MessageReceived);
    }

    public override void StopInput()
    {
    }

    public override void UpdateInput()
    {

    }

    private void MessageReceived(OSCMessage message)
    {

    }
}
