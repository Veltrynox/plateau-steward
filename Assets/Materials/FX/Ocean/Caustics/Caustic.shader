Shader "PS/CausticsProjector"
{
    Properties
    {
        [Header(Main Settings)]
        _MainTex ("Caustic Texture", 2D) = "black" {}
        [HDR] _Color("Tint Color", Color) = (1, 1, 2, 1)
        _Brightness ("Brightness", Range(0, 5)) = 1.0

        [Header(Projection)]
        _Scale("Texture Scale", Float) = 2.0
        _Speed1 ("Layer 1 Speed", Vector) = (0.05, 0.05, 0, 0)
        _Speed2 ("Layer 2 Speed", Vector) = (-0.03, 0.04, 0, 0)

        [Header(Refraction)]
        _DistortionStrength ("Distortion Strength", Range(0, 2.0)) = 0.5

        [Header(Fading)]
        _SurfaceY ("Surface Y Level", Range(-10, 10)) = 0.0
        _FadeY_min ("Fade Y Min", Range(0, -60)) = -50.0
        _FadeY_max ("Fade Y Max", Range(0, 5)) = 0.4
        _Softness("Edge Fade", Range(0.001, 0.5)) = 0.2

        [Header(FX)]
        _Aberration("Chromatic Aberration", Range(0, 0.02)) = 0.005

        [Header(Blur)]
        _BlurSteps("Blur Steps", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Cull Front
            ZTest Greater
            ZWrite Off
            Blend One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Assets/Materials/FX/Ocean/WaterCommon.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
                float2 uv         : TEXCOORD1;
                float3 rayDirWS   : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Brightness;
                float _Scale;
                float4 _Speed1;
                float4 _Speed2;
                float _DistortionStrength;
                float _SurfaceY;
                float _FadeY_min;
                float _FadeY_max;
                float _Softness;
                float _Aberration;
                float _BlurSteps;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.rayDirWS = normalize(worldPos - _WorldSpaceCameraPos);

                return output;
            }

            float3 SampleCaustics(float2 uv, int lod = 0)
            {
                float r = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv + float2(_Aberration, 0), lod).r;
                float g = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv, lod).g;
                float b = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, uv - float2(_Aberration, 0), lod).b;
                return float3(r, g, b);
            }

            float4 frag(Varyings input) : SV_Target
            {
                // --- PROJECTION LOGIC ---
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float3 worldPos = ComputeWorldSpacePosition(screenUV, rawDepth, UNITY_MATRIX_I_VP);
                float3 objectPos = TransformWorldToObject(worldPos);

                float3 bounds = abs(objectPos);
                if (bounds.x > 0.5 || bounds.y > 0.5 || bounds.z > 0.5) discard;

                float edgeFade = 1.0 - max(max(smoothstep(0.5-_Softness, 0.5, bounds.x),
                                               smoothstep(0.5-_Softness, 0.5, bounds.y)),
                                               smoothstep(0.5-_Softness, 0.5, bounds.z));

                // --- DISTORTION ---
                float3 camPos = _WorldSpaceCameraPos;
                float3 rayDir = normalize(worldPos - camPos);
                float distToWater = (_SurfaceY - camPos.y) / rayDir.y;
                float3 waterSurfacePos = camPos + rayDir * distToWater;
                float3 waveDisp, waveNormal;
                GetWaterSurfaceData(waterSurfacePos, 0, waveDisp, waveNormal);
                float2 distortion = waveNormal.xz * _DistortionStrength;
                float2 baseUV = (worldPos.xz * _Scale) + distortion;

                float steps = max(_BlurSteps, 2);
                float2 uv1 = baseUV + (_Time.y * _Speed1.xy);
                float2 uv2 = baseUV + (_Time.y * _Speed2.xy);

                // --- FADING LOGIC ---
                float invFadeMinRange = rcp(_SurfaceY - _FadeY_min);
                float invFadeMaxRange = rcp(_FadeY_max);

                float fadeMin = saturate((worldPos.y - _FadeY_min) * invFadeMinRange);
                float fadeMax = 1.0 - saturate((worldPos.y + _SurfaceY) * invFadeMaxRange);
                float fadeHeight = min(fadeMin, fadeMax);

                float3 col1 = SampleCaustics(uv1, _BlurSteps);
                float3 col2 = SampleCaustics(uv2, _BlurSteps);

                float3 finalColor = min(col1, col2);

                finalColor /= steps;

                // --- FINAL COLOR LOGIC ---
                finalColor *= _Color.rgb * _Brightness * edgeFade * fadeHeight;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}