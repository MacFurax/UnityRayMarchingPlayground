using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// TODO add music https://www.youtube.com/watch?v=Nj2sOjlMaJA
//

public class ThreeDMetaBalls : ShaderImputHandlerBase
{
    private OSCSource oscSource;
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();
    private List<Vector4> particles = new List<Vector4>(10);
    private int particlesCount = 10;
    private float fogScater = 0.1f;

    public ThreeDMetaBalls()
    {
        handlerName = "3D Metaballs";
    }

    public override void PopulateUniforms(ref Material mat)
    {
        int x = 0;
        foreach (Vector2 playerPos in posPerPlayer.Values)
        {
            if (x < particles.Count)
            {
                Vector4 v = particles[x];
                v.x = ((playerPos.x * 2) - 1) * 8.0f;
                v.y = ((playerPos.y * 2) - 1) * 8.0f;
                particles[x] = v;
            }
            x++;
        }

        mat.SetVectorArray("_Particles", particles);
        mat.SetInt("_ParticlesCount", particlesCount);
        mat.SetFloat("_FogScater", fogScater);
    }

    public override void Init()
    {
        // set config message handler
        oscSource = GetComponent<OSCSource>();
        oscSource.BindAddress("/3DMetaballs/*", MessageReceived);
    }

    public override void Close()
    {
        //base.Close();
    }

    public override void UpdateHandler()
    {
        //base.UpdateHandler();
        if (activated)
        {

        }
    }

    public void Start()
    {
        // init balls position
        for (int x = 0; x < particlesCount; x++)
        {
            Vector3 v3 = Random.onUnitSphere*2.5f;
            float v1 = Random.value;
            v1 = 0.7f;
            Vector4 v4 = new Vector4(v3.x, v3.y, v3.z/2.0f, v1);
            particles.Add(v4);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        if (activated)
        {
            for (int x = 0; x < particlesCount; x++)
            {
                Vector4 v4 = particles[x];
                Vector3 v3 = new Vector3(v4.x, v4.y, v4.z);
                v3 = Quaternion.Euler( Random.value * 15 * Time.deltaTime, 10 * Time.deltaTime, 0) * v3;
                v4 = new Vector4(v3.x, v3.y, v3.z, v4.w);
                particles[x] = v4;
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

    }

    public override void Deactivate()
    {
        base.Deactivate();

    }

    public override void NewMove(InputBase.ImputMoveEventArgs newMove)
    {
        posPerPlayer[newMove.playerId] = newMove.pos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void MessageReceived(OSCMessage message)
    {
        if (message.Address.Equals("/3DMetaballs/fogScater"))
        {
            OSCValue[] vals = message.GetValues(message.GetTypes());

            fogScater = vals[0].FloatValue; 
        }
    }
}
