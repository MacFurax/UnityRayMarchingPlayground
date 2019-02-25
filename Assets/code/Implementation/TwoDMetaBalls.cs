using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDMetaBalls : ShaderImputHandlerBase
{
    private OSCSource oscSource;
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();
    Matrix4x4 particules = new Matrix4x4();

    int maxBalls = 4;

    public TwoDMetaBalls()
    {
        handlerName = "2D Metaballs";
        // set default particules position
        //particules.SetRow(0, new Vector4(0.25f, 0.75f, 0.0f, 0.0f));
        //particules.SetRow(0, new Vector4(0.75f, 0.75f, 0.0f, 0.0f));
        //particules.SetRow(0, new Vector4(0.25f, 0.25f, 0.0f, 0.0f));
        //particules.SetRow(0, new Vector4(0.75f, 0.25f, 0.0f, 0.0f));
        particules.SetRow(0, new Vector4(0.5f, 0.5f, 0.0f, 0.0f));
        particules.SetRow(0, new Vector4(0.6f, 0.6f, 0.0f, 0.0f));
        particules.SetRow(0, new Vector4(-1.0f, -1.0f, 0.0f, 0.0f));
        particules.SetRow(0, new Vector4(-1.0f, -1.0f, 0.0f, 0.0f));

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    public override void PopulateUniforms(ref Material mat)
    {
        int count = 0;
        foreach ( Vector2 vals in posPerPlayer.Values)
        {
            if (count > maxBalls) break;
            particules[count, 0] = vals.x;
            particules[count, 1] = vals.y;
            count++;
        }
        mat.SetMatrix("_Particles", particules);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Init()
    {
        // set config message handler
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/2DMetaballs/*", MessageReceived);

    }

    /// <summary>
    /// 
    /// </summary>
    public override void Close()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public override void UpdateHandler()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void MessageReceived(OSCMessage message)
    {
        // config message handlers
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newMove"></param>
    public override void NewMove(InputBase.ImputMoveEventArgs newMove)
    {
        //Debug.Log("TwoDMetaBalls::NewMove - " + newMove.pos);
        posPerPlayer[newMove.playerId] = newMove.pos;
    }
}
