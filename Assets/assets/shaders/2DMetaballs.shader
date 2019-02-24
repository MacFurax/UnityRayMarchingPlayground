Shader "My Shaders/2DMetaballs"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            
            #include "UnityCG.cginc"

            #pragma target 3.0

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Resolution;

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

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x *= _ScreenParams.x/ _ScreenParams.y;

                fixed4 col = float4(i.uv.x, i.uv.y, 0.0, 1.0);

                return col;
            }
            ENDCG
        }
    }
}
