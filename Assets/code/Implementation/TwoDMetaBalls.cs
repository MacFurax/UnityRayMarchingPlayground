using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDMetaBalls : ShaderImputHandlerBase
{
    private OSCSource oscSource;
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();
    
    private List<Vector4> particles = new List<Vector4>(10);
    private int particlesCount = 2;

    int maxBalls = 4;

    public TwoDMetaBalls()
    {
        handlerName = "2D Metaballs";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    public override void PopulateUniforms(ref Material mat)
    {
        int x = 0;
        foreach (Vector2 playerPos in posPerPlayer.Values)
        {
            if (x < particles.Count)
            {
                Vector4 v = particles[x];
                v.x = playerPos.x;
                v.y = playerPos.y;
                particles[x] = v;
            }
            x++;
        }

        mat.SetVectorArray("_Particles", particles);
        mat.SetInt("_ParticlesCount", particlesCount);
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
        
        //Debug.Log("TwoDMetaBalls::NewMove - player ["+ newMove.playerId+ "] " + newMove.pos);
        posPerPlayer[newMove.playerId] = newMove.pos;
    }

    public void Start()
    {
        // init balls position
        for (int x = 0; x < particlesCount; x++)
        {
            Vector2 v2 = Random.insideUnitCircle;
            float v1 = 0.1f + (Random.value/2.0f);
            Vector4 v4 = new Vector4(v2.x, v2.y, v1, 0.0f);
            particles.Add(v4);
        }

    }
}
