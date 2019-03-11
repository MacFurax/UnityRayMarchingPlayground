using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDMetaBalls : ShaderImputHandlerBase
{

    private List<Vector4> particles = new List<Vector4>(10);
    private int particlesCount = 10;

    public ThreeDMetaBalls()
    {
        handlerName = "3D Metaballs";
    }

    public override void PopulateUniforms(ref Material mat)
    {
        //base.PopulateUniforms(ref mat);
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

    public override void Activate()
    {
        base.Activate();

    }

    public override void Deactivate()
    {
        base.Deactivate();

    }
}
