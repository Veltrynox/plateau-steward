Shader "SubnauticaClone/UnderwaterShader"
{
    Properties
    {
        _WaterColor("Water Fog Color", Color) = (0.0, 0.5, 0.7, 1)
        _TintColor("Final Tint", Color) = (0.8, 0.9, 1.0, 1)
        _FogDensity("Fog Density", Float) = 0.05
        _DesaturateStrength("Desaturation Strength", Float) = 0.03
        _WaterLevel("Water Level", Float) = 0
        _WaterFadeDistance("Fade Distance", Float) = 5

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off
        Pass
        {
            Name "UnderwaterShader"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float4 _WaterColor;
            float4 _TintColor;
            float _FogDensity;
            float _DesaturateStrength;
            float _WaterLevel;
            float _WaterFadeDistance;

            float4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // sample the texture using the SAMPLE_TEXTURE2D_X_LOD
                float2 uv = input.texcoord.xy;
                half4 color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, uv, _BlitMipLevel);

                float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float depth = LinearEyeDepth(rawDepth, _ZBufferParams);

                float3 worldPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);

                // Determine blend factor based on Y < 0
                float blend = saturate(-worldPos.y / 5.0); 

                // Desaturation
                float grayscale = dot(color.rgb, float3(0.299, 0.587, 0.114));
                float desat = saturate(depth * _DesaturateStrength);
                color.rgb = lerp(color.rgb, grayscale.xxx, desat * blend);

                // Fog
                float fog = saturate(depth * _FogDensity) * blend;
                color.rgb = lerp(color.rgb, _WaterColor.rgb, fog);

                // Final tint modulation
                color.rgb *= lerp(float3(1,1,1), _TintColor.rgb, blend);

                return color;

            }


            ENDHLSL
        }
    }
}