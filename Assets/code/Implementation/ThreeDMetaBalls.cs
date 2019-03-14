using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDMetaBalls : ShaderImputHandlerBase
{
    private Dictionary<string, Vector2> posPerPlayer = new Dictionary<string, Vector2>();
    private List<Vector4> particles = new List<Vector4>(10);
    private int particlesCount = 10;

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
    }

    public override void Init()
    {
        //base.Init();

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
        for (int x = 0; x < 10; x++)
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
}
