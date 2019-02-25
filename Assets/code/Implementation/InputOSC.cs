using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class InputOSC : InputBase
{

    public class InputState
    {
        string inputId = "";
        bool trigger = false;
        float x = 0.0f;
        float y = 0.0f;

        public InputState()
        {
        }

        public InputState(InputState src)
        {
            this.inputId = src.inputId;
            this.trigger = src.trigger;
            this.x = src.x;
            this.y = src.y;
        }
    }

    OSCSource oscSource;
    Queue<InputState> inputs = new Queue<InputState>();
    InputState lastQueued = null;

    public Queue<InputState> Inputs { get => inputs; }

    public InputOSC()
    {
        inputName = "OSC input";
    }

    public override void Init()
    {
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/inputs/*", MessageReceived);
    }

    public override void Close()
    {
    }

    public override void UpdateInput()
    {

    }

    private void MessageReceived(OSCMessage message)
    {
        if (message.Address.Contains("/xy"))
        {
            InputMove(GeneratePlayerId(message), GetPos(message));
        }
        else if (message.Address.Contains("/push"))
        {
            Debug.Log("Get new trigger from [" + message.Address + "]");
        }
    }

    public void ClearInputs()
    {
        inputs.Clear();
    }

    private string GeneratePlayerId(OSCMessage message)
    {
        string address = message.Address;
        if (address.Contains("/xy"))
        {
            string id = address.Substring(11);
            return id;
        }
        else if (address.Contains("/push"))
        {
            string id = address.Substring(13);
            return id;
        }
        return "01";
    }

    private Vector2 GetPos(OSCMessage message)
    {
        OSCValueType[] types = { OSCValueType.Float };
        OSCValue[] vals = message.GetValues(types);

        return new Vector2(vals[0].FloatValue, vals[1].FloatValue);
    }
}
