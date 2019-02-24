using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDMetaBalls : ShaderImputHandlerBase
{
    private OSCSource oscSource;
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();
    Matrix4x4 particules = new Matrix4x4();

    public TwoDMetaBalls()
    {
        handlerName = "2D Metaballs";
    }

    public override void PopulateUniforms(ref Material mat)
    {
        
    }

    public override void Init()
    {
        // set config message handler
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/2DMetaballs/*", MessageReceived);

    }

    public override void Close()
    {
        
    }

    public override void UpdateHandler()
    {
        
    }

    private void MessageReceived(OSCMessage message)
    {
        // config message handlers
    }

    public override void NewMove(InputBase.ImputMoveEventArgs newMove)
    {
        Debug.Log("TwoDMetaBalls::NewMove - Move player [" + newMove.playerId + "] pos [" + newMove.pos + "]");
        posPerPlayer[newMove.playerId] = newMove.pos;
    }
}
