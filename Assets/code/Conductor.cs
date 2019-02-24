using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public class ActivateShaderEventArgs : EventArgs
    {
        public Shader newShader;

        public ActivateShaderEventArgs(Shader newShader)
        {
            this.newShader = newShader;
        }
    }

    private InputBase[] inputs;
    private ShaderImputHandlerBase[] shaderImputHandlers;
    private int activeShaderHandlerIdx = -1;
    private int previousActiveShaderHandlerIdx = 0;
    private ShaderImputHandlerBase activeShaderHandler;

    private OSCSource oscSource;

    private bool probRep = true;

    private List<string> shaderNames = new List<string>();

    public ShaderImputHandlerBase ActiveShaderHandler { get => activeShaderHandler; }

    public event EventHandler<ActivateShaderEventArgs> OnNewShaderActivated;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    public void PopulateUniforms(Material mat)
    {
        if (activeShaderHandlerIdx >= 0)
        {
            activeShaderHandler.PopulateUniforms(ref mat);
        }
    }

    // Start is called before the first frame update
    public void Init()
    {
        // get OSC source to receive Conductor remote config
        oscSource = GetComponent<OSCSource>();
        oscSource.Init();

        oscSource.BindAddress("/conductor/*", MessageReceived);
    
        // load handlers as components
        shaderImputHandlers = GetComponents<ShaderImputHandlerBase>();

        foreach (ShaderImputHandlerBase sh in shaderImputHandlers)
        {
            shaderNames.Add(sh.HandlerName);

            Debug.Log("Available Shader handler: " + sh.HandlerName);

            sh.Init();
        }

        if (shaderImputHandlers.Length > 0)
        {
            activeShaderHandlerIdx = 0;
            activeShaderHandler = shaderImputHandlers[activeShaderHandlerIdx];
            OnNewShaderActivated?.Invoke(this, new ActivateShaderEventArgs(activeShaderHandler._shader));
        }
        else
        {
            activeShaderHandlerIdx = -1;
            activeShaderHandler = null;
        }

        // load imputs
        inputs = GetComponents<InputBase>();
        foreach ( InputBase ib in inputs)
        {
            Debug.Log("Available input: " + ib.InputName);
            ib.Init();

            ib.OnInputMove += Ib_OnImputMove;
            ib.OnTriggerChanged += Ib_OnTriggerChanged;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Ib_OnTriggerChanged(object sender, InputBase.ImputTrigger e)
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Ib_OnImputMove(object sender, InputBase.ImputMoveEventArgs e)
    {
        if (activeShaderHandlerIdx >= 0)
        {
            activeShaderHandler.NewMove(e);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        foreach (InputBase ib in inputs)
        {
            ib.Close();

            ib.OnInputMove -= Ib_OnImputMove;
            ib.OnTriggerChanged -= Ib_OnTriggerChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void MessageReceived(OSCMessage message)
    {
        Debug.Log("OSC message received " + message.Address);
        
        // receive prob from Open Stage Control
        if (message.Address.Equals("/conductor/probOSC"))
        {
            ReplyProbOpenStageControl(message);
        }
        else if (message.Address.StartsWith("/conductor/shader"))
        {
            string shaderIdStr = message.Address.Substring(17);
            int shaderId = -1;
            if (!int.TryParse(shaderIdStr, out shaderId))
            {
                Debug.Log("Faile to parse shader id [" + shaderIdStr + "]");
            }
            else
            {
                Debug.Log("Activate Shader " + shaderId);
                // -1 because shader ID in UI start at 1
                ActivateShader(shaderId-1, message);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="shadeId"></param>
    private void ActivateShader(int shaderId, OSCMessage message)
    {
        if (shaderId >= shaderNames.Count)
        {
            return;
        }

        previousActiveShaderHandlerIdx = activeShaderHandlerIdx;
        activeShaderHandlerIdx = shaderId;

        activeShaderHandler = shaderImputHandlers[activeShaderHandlerIdx];

        OnNewShaderActivated?.Invoke(this, new ActivateShaderEventArgs(activeShaderHandler._shader));

        UpdateLedStatus(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void ReplyProbOpenStageControl(OSCMessage message)
    {
        probRep = !probRep;
        OSCBundle bundle = new OSCBundle();

        // prob 
        var msg = new OSCMessage("/conductor/ledProb");
        msg.AddValue(OSCValue.Float(probRep ? 1.0f : 0.0f));
        bundle.AddPacket(msg);
                
        oscSource.Send(message.Ip.ToString(), bundle);

        SendShaderListOpenStageControl(message);

        UpdateLedStatus(message, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void SendShaderListOpenStageControl(OSCMessage message)
    {
        OSCBundle bundle = new OSCBundle();

        List<string> oscNames = new List<string>();
        for (int x = 0; x < 10; x++)
        {
            if (x < shaderNames.Count)
            {
                oscNames.Add(shaderNames[x]);
            }
            else
            {
                oscNames.Add("---");
            }
        }

        // list of shader handlers and led state
        int count = 1;
        foreach (string name in oscNames)
        {
            var msg = new OSCMessage("/EDIT/MERGE");
            msg.AddValue(OSCValue.String("shader" + count));
            msg.AddValue(OSCValue.String("{'label':'" + name + "'}"));
            bundle.AddPacket(msg);
            
            count++;
        }

        oscSource.Send(message.Ip.ToString(), bundle);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void UpdateLedStatus(OSCMessage message, bool all = false)
    {
        OSCBundle bundle = new OSCBundle();

        if (!all)
        {
            // only send active new on ad disactive previous
            if (activeShaderHandlerIdx != previousActiveShaderHandlerIdx)
            {
                int count = 1;
                foreach (string name in shaderNames)
                {

                    if (count == activeShaderHandlerIdx + 1)
                    {
                        var msg = new OSCMessage("/conductor/shaderLed" + count);
                        msg.AddValue(OSCValue.Float(1.0f));
                        bundle.AddPacket(msg);
                    }

                    if (count == previousActiveShaderHandlerIdx + 1)
                    {
                        var msg = new OSCMessage("/conductor/shaderLed" + count);
                        msg.AddValue(OSCValue.Float(0.0f));
                        bundle.AddPacket(msg);
                    }

                    count++;
                }
            }
            else
            {
                var msg = new OSCMessage("/conductor/shaderLed" + (activeShaderHandlerIdx + 1));
                msg.AddValue(OSCValue.Float(1.0f));
                bundle.AddPacket(msg);
            }
        }
        else
        {
            // send all led status
            int count = 1;
            foreach (string name in shaderNames)
            {

                if (count == activeShaderHandlerIdx + 1)
                {
                    var msg = new OSCMessage("/conductor/shaderLed" + count);
                    msg.AddValue(OSCValue.Float(1.0f));
                    bundle.AddPacket(msg);
                }
                else
                {
                    var msg = new OSCMessage("/conductor/shaderLed" + count);
                    msg.AddValue(OSCValue.Float(0.0f));
                    bundle.AddPacket(msg);
                }
                
                count++;
            }
        }

        

        oscSource.Send(message.Ip.ToString(), bundle);

    }
}
