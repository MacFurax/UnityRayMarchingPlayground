using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasmama : ShaderImputHandlerBase
{
    private OSCSource oscSource;
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();

    private List<Vector4> particles = new List<Vector4>(10);
    private int particlesCount = 10;

    public Texture2D[] _textures;

    public Plasmama()
    {
        handlerName = "Plasmama";
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void Init()
    {
        // set config message handler
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/Plasmama/*", MessageReceived);
    }

    public override void NewMove(InputBase.ImputMoveEventArgs newMove)
    {
        posPerPlayer[newMove.playerId] = newMove.pos;
    }

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

    public override void UpdateHandler()
    {
        base.UpdateHandler();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void MessageReceived(OSCMessage message)
    {
        // config message handlers
    }
}
