using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{

    private InputBase[] inputs;
    private ShaderImputHandlerBase[] shaderImputHandlers;
    private int activeShaderHandlerIdx = 0;
    private ShaderImputHandlerBase activeSHaderHandler;

    private OSCSource oscSource;

    private bool probRep = true;

    private List<string> shaderNames = new List<string>();

    public void PopulateUniforms(Material mat)
    {

    }

    // Start is called before the first frame update
    void Start()
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
            activeSHaderHandler = shaderImputHandlers[activeShaderHandlerIdx];
        }
        else
        {
            activeShaderHandlerIdx = -1;
            activeSHaderHandler = null;
        }

        // load imputs
        inputs = GetComponents<InputBase>();
        foreach ( InputBase ib in inputs)
        {
            Debug.Log("Available input: " + ib.InputName);
            ib.Init();

            ib.OnImputMove += Ib_OnImputMove;
            ib.OnTriggerChanged += Ib_OnTriggerChanged;
        }

    }

    private void Ib_OnTriggerChanged(object sender, InputBase.ImputTrigger e)
    {
        
    }

    private void Ib_OnImputMove(object sender, InputBase.ImputMoveEventArgs e)
    {
        
    }

    private void OnDisable()
    {
        foreach (InputBase ib in inputs)
        {
            ib.Close();

            ib.OnImputMove -= Ib_OnImputMove;
            ib.OnTriggerChanged -= Ib_OnTriggerChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MessageReceived(OSCMessage message)
    {
        //Debug.Log(message);
        //float tmp = 0;

        // receive prob from Open Stage Control
        if (message.Address.Equals("/conductor/probOSC"))
        {
            ReplyProbOpenStageControl(message);
        }

    }

    private void ReplyProbOpenStageControl(OSCMessage message)
    {
        probRep = !probRep;
        OSCBundle bundle = new OSCBundle();

        // prob 
        var msg = new OSCMessage("/conductor/ledProb");
        msg.AddValue(OSCValue.Float(probRep ? 1.0f : 0.0f));
        bundle.AddPacket(msg);

        // list of shader handlers
        int count = 1;
        foreach ( string name in shaderNames )
        {
            msg = new OSCMessage("/EDIT");
            msg.AddValue(OSCValue.String("shaderList" + count));
            msg.AddValue(OSCValue.String("{'label':'"+name+"', 'color' : 'green'}"));
            bundle.AddPacket(msg);
            count++;
        }

        oscSource.Send(message.Ip.ToString(), bundle);
    }
}
