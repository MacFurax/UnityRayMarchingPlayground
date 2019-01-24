Shader "Unlit/externalParticles"
{
  Properties
  {
      _MainTex("Texture", 2D) = "white" {}
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
        // Compile one version of the shader with performance debugging
        // This way we can simply set a shader keyword to test perf
        #pragma multi_compile __ DEBUG_PERFORMANCE
        // You may need to use an even later shader model target, depending on how many instructions you have
        // or if you need variable-length for loops.
        #pragma target 3.0

        #include "UnityCG.cginc"

        uniform sampler2D _CameraDepthTexture;

        // These are are set by our script (see RaymarchGeneric.cs)
        uniform sampler2D _MainTex;
        uniform float4 _MainTex_TexelSize;

        uniform float4x4 _CameraInvViewMatrix;
        uniform float4x4 _FrustumCornersES;
        uniform float4 _CameraWS;

        uniform float3 _LightDir;
        uniform float4x4 _MatTorus_InvModel;
        uniform sampler2D _ColorRamp_Material;
        uniform sampler2D _ColorRamp_PerfMap;

        uniform float2 _Resolution;
        uniform float4x4 _PartPos;

        uniform float _DrawDistance;

        // define for ShaderToy to Unity compatibility
        #define vec4 float4
        #define vec3 float3
        #define vec2 float2
        #define iTime _Time.y

        struct appdata
        {
          // Remember, the z value here contains the index of _FrustumCornersES to use
          float4 vertex : POSITION;
          float2 uv : TEXCOORD0;
      };

      struct v2f
      {
          float4 pos : SV_POSITION;
          float2 uv : TEXCOORD0;
          float3 ray : TEXCOORD1;
      };

      v2f vert(appdata v)
      {
          v2f o;

          // Index passed via custom blit function in RaymarchGeneric.cs
          half index = v.vertex.z;
          v.vertex.z = 0.1;

          o.pos = UnityObjectToClipPos(v.vertex);
          o.uv = v.uv.xy;

          #if UNITY_UV_STARTS_AT_TOP
          if (_MainTex_TexelSize.y < 0)
              o.uv.y = 1 - o.uv.y;
          #endif

          // Get the eyespace view ray (normalized)
          o.ray = _FrustumCornersES[(int)index].xyz;
          // Dividing by z "normalizes" it in the z axis
          // Therefore multiplying the ray by some number i gives the viewspace position
          // of the point on the ray with [viewspace z]=i
          o.ray /= abs(o.ray.z);

          // Transform the ray from eyespace to worldspace
          o.ray = mul(_CameraInvViewMatrix, o.ray);

          return o;
      }





      void mainImage(out vec4 fragColor, in vec2 fragCoord)
      {
        //fragColor = vec4(fragCoord.x, fragCoord.y, 0.0, 1.0);

        // Normalized pixel coordinates (from -.5 to .5)
        fragCoord -= 0.5;
        fragCoord *= 2.0;

        vec2 uv = fragCoord;
        uv.x *= _Resolution.x / _Resolution.y;

        // dein circle position and radius
        vec3 cercle1 = vec3(_PartPos[0][0], _PartPos[1][0], _PartPos[3][0]);
        vec3 cercle2 = vec3(_PartPos[0][1], _PartPos[1][1], _PartPos[3][1]);
        vec3 cercle3 = vec3(_PartPos[0][2], _PartPos[1][2], _PartPos[3][2]);
        vec3 cercle4 = vec3(_PartPos[0][3], _PartPos[1][3], _PartPos[3][3]);
       
        //cercle1.x += (sin(iTime) + 1.) / 2.;
        //cercle1.y -= (sin(iTime) + 1.) / 2.;

        // get distance from circle center to uv
        float d1 = distance(cercle1.xy, uv);
        float d2 = distance(cercle2.xy, uv);
        float d3 = distance(cercle3.xy, uv);
        float d4 = distance(cercle4.xy, uv);

        // create a gradiant from circle center to radius
        float c1 = smoothstep(cercle1.z + .2, cercle1.z - 0.2, d1);
        float c2 = smoothstep(cercle2.z + .2, cercle2.z - 0.2, d2);
        float c3 = smoothstep(cercle3.z + .2, cercle3.z - 0.2, d3);
        float c4 = smoothstep(cercle4.z + .2, cercle4.z - 0.2, d4);

        // sum 
        float c = (c1 + c2 + c3 + c4)*1.5;
        //float c = c1;

        // treshold, comment it to see blur
        c = (c > .4 ? 1. : .2);

        // color
        //float s = (sin(iTime) + 1.) * .5;
        //vec3 col = vec3((uv.x + .5) * c, (uv.y + .5) * c, s / 2. + .4);

        vec3 col = vec3(c, c, c);

        // Output to screen
        fragColor = vec4(col, 1.0);
      }

      fixed4 frag(v2f i) : SV_Target
      {
        // ray direction
        float3 rd = normalize(i.ray.xyz);

        // ray origin (camera position)
        float3 ro = _CameraWS;

        float2 duv = i.uv;

        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
            duv.y = 1 - duv.y;
        #endif

        // Convert from depth buffer (eye space) to true distance from camera
        // This is done by multiplying the eyespace depth by the length of the "z-normalized"
        // ray (see vert()).  Think of similar triangles: the view-space z-distance between a point
        // and the camera is proportional to the absolute distance.
        float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, duv).r);
        depth *= length(i.ray);

        // i.uv is normalised 0 <> 1, from lower left corner

        vec4 fragColor;
        mainImage(fragColor, i.uv);

        // return fixed4(i.uv.x, i.uv.y, 0.0 , 1.0);
         return fragColor;
      }
      ENDCG
      }
  }
}
