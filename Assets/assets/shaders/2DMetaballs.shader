Shader "My Shaders/2DMetaballs"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplacementTex("Displacement", 2D) = "white" {}
        
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
  
            #pragma vertex vert
            #pragma fragment frag
            
            #define mod fmod
            #define vec3 float3
            
            #include "UnityCG.cginc"

            #pragma target 5.0

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float3 ray : TEXCOORD1;
            };

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float2 _Resolution;

            uniform float4 _Particles[10];
            uniform int _ParticlesCount;
            uniform sampler2D _DisplacementTex;
           
            v2f vert (appdata v)
            {
                v2f o;

                // Index passed via custom blit function in RaymarchGeneric.cs
                half index = v.vertex.z;
                v.vertex.z = 0.1;
                v.vertex.w = 0.0;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                return o;
            }

            float3 HUEtoRGB(in float H)
            {
              float R = abs(H * 6 - 3) - 1;
              float G = 2 - abs(H * 6 - 2);
              float B = 2 - abs(H * 6 - 4);
              return saturate(float3(R, G, B));
            }

            float3 HSLtoRGB(in float3 HSL)
            {
              float3 RGB = HUEtoRGB(HSL.x);
              float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
              return (RGB - 0.5) * C + HSL.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float r = _ScreenParams.x / _ScreenParams.y;
                i.uv.x *= r; // apply screen ratio

                float s = 0.0f;
                float3 spcol = float3(0.0, 0.0, 0.0);

                fixed4 col = float4(i.uv.x, i.uv.y, 1.0, 1.0);

                for (int aa = 0; aa < _ParticlesCount; aa++)
                {
                  float2 p = _Particles[aa].xy;
                  p.x *= r; // apply screen ratio

                  // distance gradiant limited between 0 and 1
                  // and circle radius
                  float d = smoothstep(_Particles[aa].z, 0.001, distance(i.uv, p.xy));
                  
                  s += d; // sum particles distance field to get praticle merge zone 

                  float3 t = HUEtoRGB((float)aa/ _ParticlesCount);
                  spcol = (spcol + t) * 0.8;

                }

                s = smoothstep(0.5, 0.52, s);

                col = float4(s*spcol.r, s*spcol.g, s*spcol.b, 1.0);
                //col = float4(s, s, s, 1.0);

                return col;
            }
            ENDCG
        }
    }
}
