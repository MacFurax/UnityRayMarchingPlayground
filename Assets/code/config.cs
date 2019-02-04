using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConfig
{
    public Vector3 bgColor = new Vector3(0.3f, 0.3f, 0.0f);
    public Vector3[] ballsColor = new Vector3[4] {
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 0.0f)
    };

    public Vector4[] ballsData = new Vector4[4] {
        new Vector4(-0.5f, 0.5f, 0.0f, 0.2f),
        new Vector4(0.5f, 0.5f, 0.0f, 0.2f),
        new Vector4(0.5f, -0.5f, 0.0f, 0.2f),
        new Vector4(-0.5f, -0.5f, 0.0f, 0.2f)
    };
    
    
}
