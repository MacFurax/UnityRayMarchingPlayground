Shader "Hidden/basic"
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
            #include "DistanceFunc.cginc"

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

            // This is the distance field function.  The distance field represents the closest distance to the surface
            // of any object we put in the scene.  If the given point (point p) is inside of an object, we return a
            // negative answer.
            // return.x: result of distance field
            // return.y: material data for closest object
            float2 map(float3 p) {
                // Apply inverse model matrix to point when sampling torus
                // This allows for more complex transformations/animation on the torus
                float3 torus_point = mul(_MatTorus_InvModel, float4(p,1)).xyz;
                float2 d_torus = float2(sdTorus(torus_point, float2(1, 0.2)), 0.5);

                float2 d_box = float2(sdBox(p - float3(-3,0,0), float3(1.25,1.5,1.5)), 0.25);
                float2 d_sphere = float2(sdSphere(p - float3(3,0,0), 1), 0.75);

                /*float2 ret = opU_mat(d_torus, d_box);
                ret = opU_mat(ret, d_sphere);*/

                float2 ret = opSmoothU_mat(d_torus, d_box, 0.5);
                ret = opSmoothU_mat(ret, d_sphere, 0.9);

                return ret;
            }

            float3 calcNormal(in float3 pos)
            {
                const float2 eps = float2(0.001, 0.0);
                // The idea here is to find the "gradient" of the distance field at pos
                // Remember, the distance field is not boolean - even if you are inside an object
                // the number is negative, so this calculation still works.
                // Essentially you are approximating the derivative of the distance field at this point.
                float3 nor = float3(
                    map(pos + eps.xyy).x - map(pos - eps.xyy).x,
                    map(pos + eps.yxy).x - map(pos - eps.yxy).x,
                    map(pos + eps.yyx).x - map(pos - eps.yyx).x);
                return normalize(nor);
            }

            // Raymarch along given ray
            // ro: ray origin
            // rd: ray direction
            // s: unity depth buffer
            fixed4 raymarch(float3 ro, float3 rd, float s) {
                fixed4 ret = fixed4(0,0,0,0);

                const int maxstep = 64;
                float t = 0; // current distance traveled along ray
                for (int i = 0; i < maxstep; ++i) {
                    // If we run past the depth buffer, or if we exceed the max draw distance,
                    // stop and return nothing (transparent pixel).
                    // this way raymarched objects and traditional meshes can coexist.
                    if (t >= s || t > _DrawDistance) {
                        ret = fixed4(0, 0, 0, 0);
                        break;
                    }

                    float3 p = ro + rd * t; // World space position of sample
                    float2 d = map(p);		// Sample of distance field (see map())

                    // If the sample <= 0, we have hit something (see map()).
                    if (d.x < 0.001) {
                        float3 n = calcNormal(p);
                        float light = dot(-_LightDir.xyz, n);
                        ret = fixed4(tex2D(_ColorRamp_Material, float2(d.y,0)).xyz * light, 1);
                        break;
                    }

                    // If the sample > 0, we haven't hit anything yet so we should march forward
                    // We step forward by distance d, because d is the minimum distance possible to intersect
                    // an object (see map()).
                    t += d;
                }

                return ret;
            }

            // Modified raymarch loop that displays a heatmap of ray sample counts
            // Useful for performance testing and analysis
            // ro: ray origin
            // rd: ray direction
            // s: unity depth buffer
            fixed4 raymarch_perftest(float3 ro, float3 rd, float s) {
                const int maxstep = 64;
                float t = 0; // current distance traveled along ray

                for (int i = 0; i < maxstep; ++i) {
                    float3 p = ro + rd * t; // World space position of sample
                    float2 d = map(p);      // Sample of distance field (see map())

                    // If the sample <= 0, we have hit something (see map()).
                    // If t > drawdist, we can safely bail because we have reached the max draw distance
                    if (d.x < 0.001 || t > _DrawDistance) {
                        // Simply return the number of steps taken, mapped to a color ramp.
                        float perf = (float)i / maxstep;
                        return fixed4(tex2D(_ColorRamp_PerfMap, float2(perf, 0)).xyz, 1);
                    }

                    t += d;
                }
                // By this point the loop guard (i < maxstep) is false.  Therefore
                // we have reached maxstep steps.
                return fixed4(tex2D(_ColorRamp_PerfMap, float2(1, 0)).xyz, 1);
            }

            void mainImage(out vec4 fragColor, in vec2 fragCoord)
            {
                //fragColor = vec4(fragCoord.x, fragCoord.y, 0.0, 1.0);
                // Normalized pixel coordinates (from -.5 to .5)
                fragCoord -= 0.5;
                vec2 uv = fragCoord;
                uv.x *= _Resolution.x / _Resolution.y;
                
                // dein circle position and radius
                vec3 cercle1 = vec3(-.2, .01, .07);
                vec3 cercle2 = vec3(.2, -.1, .1);
                vec3 cercle3 = vec3(0., 0., .2);
                vec3 cercle4 = vec3(-0.3, -0.3, .3);
                vec3 cercle5 = vec3(0., 0., .07);

                // animate circle pos and radius
                cercle1.x += sin((iTime + 0.) / 4. + sin(iTime + 12.)) / 4.;
                cercle1.y += cos(iTime + cos(iTime*2.)) / 4.;

                cercle2.x += cos(iTime / 4. + 150.) / 2. - sin(iTime / 4.) - .2;
                cercle2.y += (sin(iTime + 10.) / 4.) - cos(iTime) / 4.;

                cercle3.z += sin((iTime - 0.) / 4.) / 16.;
                cercle3.x += sin(iTime / 2.)*.6;
                cercle3.y += sin(iTime*2.)*.10;

                cercle4.z = .21 + sin(iTime / 4.1) / 16.;
                cercle4.x += sin(iTime / 4.) / 2.;
                cercle4.y += sin((iTime + 0.) / 16.) / 2.;

                cercle5.x += sin(iTime / 4.) / 1.3;
                cercle5.y += sin(iTime / 2.) / 3.;

                // get distance from circle center to uv
                float d1 = distance(cercle1.xy, uv);
                float d2 = distance(cercle2.xy, uv);
                float d3 = distance(cercle3.xy, uv);
                float d4 = distance(cercle4.xy, uv);
                float d5 = distance(cercle5.xy, uv);

                // create a gradiant from circle center to radius
                float c1 = smoothstep(cercle1.z + .2, cercle1.z - 0.2, d1);
                float c2 = smoothstep(cercle2.z + .2, cercle2.z - 0.2, d2);
                float c3 = smoothstep(cercle3.z + .2, cercle3.z - 0.2, d3);
                float c4 = smoothstep(cercle4.z + .2, cercle4.z - 0.2, d4);
                float c5 = smoothstep(cercle5.z + .2, cercle5.z - 0.2, d5);

                // sum 
                float c = (c1 + c2 + c3 + c4 + c5)*1.5;

                // treshold, comment it to see blur
                c = (c > .4 ? 1. : .2);

                // color
                float s = (sin(iTime) + 1.) * .5;
                vec3 col = vec3((uv.x + .5) * c, (uv.y + .5) * c, s / 2. + .4);

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
