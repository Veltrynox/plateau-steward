Shader "PS/WaterlineEffect"
{
    Properties
    {
        _WaterColor ("Underwater Color", Color) = (0, 0.4, 0.7, 1)
        _DistortionStrength ("Refraction Strength", Range(0, 0.1)) = 0.02
        _FogDensity ("Fog Density", Range(0, 1)) = 0.15
        _WaterlineColor ("Waterline Border Color", Color) = (0, 0.05, 0.1, 0.8)
        _WaterlineThickness ("Waterline Thickness", Range(0.001, 0.1)) = 0.02
        _DebugMode ("Debug Mode", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "WaterlinePass"
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Assets/Materials/FX/Ocean/WaterCommon.hlsl"

            // Globals
            TEXTURE2D_X(_BlitTexture);

            float4 _WaterColor;
            float _DistortionStrength;
            float _FogDensity;
            float4 _WaterlineColor;
            float _WaterlineThickness;
            float _DebugMode;


            struct Attributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 nearPlaneWorldPos : TEXCOORD1; 
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord = uv;

                float4 clipPos = float4(uv * 2.0 - 1.0, UNITY_NEAR_CLIP_VALUE, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                    if (_ProjectionParams.x < 0)
                    clipPos.y *= -1;
                #endif

                float4 worldPos = mul(unity_MatrixInvVP, clipPos);

                output.nearPlaneWorldPos = worldPos.xyz / worldPos.w;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 nearPlanePos = input.nearPlaneWorldPos;

                float3 displacement;
                float3 normal;
                GetWaterSurfaceData(nearPlanePos, 0, displacement, normal);
                
                float diff = displacement.y - nearPlanePos.y;
                
                float mask = smoothstep(-0.02, 0.05, diff);
                float sharp_mask = smoothstep(0, 0.002, diff);

                // Waterline Logic
                float distFromSurface = abs(diff);
                float solidPart = _WaterlineThickness * 0.1;
                float lineStrength = 1.0 - smoothstep(solidPart, _WaterlineThickness, distFromSurface);

                // Distortion
                float edge = mask * (1.0 - mask) * 4.0; 
                float2 distortion = float2(0, edge * _DistortionStrength);
                float2 distortedUV = input.texcoord + distortion;
                
                // Scene Sampling
                half4 sceneColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV);
                
                // Depth and Fog
                float depthRaw = SampleSceneDepth(distortedUV);
                float depthLinear = LinearEyeDepth(depthRaw, _ZBufferParams);
                float fogFactor = saturate(depthLinear * _FogDensity);
                half3 underwaterColor = lerp(sceneColor.rgb, _WaterColor.rgb, fogFactor);
                
                half3 finalColor = lerp(sceneColor.rgb, underwaterColor, sharp_mask);

                finalColor = lerp(finalColor, _WaterlineColor.rgb, lineStrength * _WaterlineColor.a);
                
                // Debug
                if (_DebugMode == 1)
                {
                    half4 debugCol = lerp(half4(1,0,0,1), half4(0,1,0,1), sharp_mask);
                    float4 clipPos = float4(input.texcoord * 2.0 - 1.0, 0.4, 1.0);
                    clipPos.y *= -1;

                    float4 viewPos = mul(unity_CameraInvProjection, clipPos);
                    viewPos /= viewPos.w;

                    float3 test = mul(unity_CameraToWorld, viewPos).xyz;
                    return half4(test, 0);
                }

                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}