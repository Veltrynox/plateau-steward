Shader "SubnauticaClone/UnderwaterShader"
{
    Properties
    {
        _WaterColor("Water Fog Color", Color) = (0.0, 0.5, 0.7, 1)
        _TintColor("Final Tint", Color) = (0.8, 0.9, 1.0, 1)
        _FogDensity("Fog Density", Float) = 0.05
        _DesaturateStrength("Desaturation Strength", Float) = 0.03
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

            #pragma vertex Vert
            #pragma fragment Frag

            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            float4 _WaterColor;
            float4 _TintColor;
            float _FogDensity;
            float _DesaturateStrength;

            // Out frag function takes as input a struct that contains the screen space coordinate we are going to use to sample our texture. It also writes to SV_Target0, this has to match the index set in the UseTextureFragment(sourceTexture, 0, …) we defined in our render pass script.   
            float4 Frag(Varyings input) : SV_Target0
            {
                // this is needed so we account XR platform differences in how they handle texture arrays
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // sample the texture using the SAMPLE_TEXTURE2D_X_LOD
                float2 uv = input.texcoord.xy;
                half4 color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, uv, _BlitMipLevel);

                float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float depth = LinearEyeDepth(rawDepth, _ZBufferParams);

                // ---- Desaturation
                float grayscale = dot(color.rgb, float3(0.299, 0.587, 0.114));
                float desat = saturate(depth * _DesaturateStrength);
                color.rgb = lerp(color.rgb, grayscale.xxx, desat);

                // ---- Fog (water absorption)
                float fog = saturate(depth * _FogDensity);
                color.rgb = lerp(color.rgb, _WaterColor.rgb, fog);

                // ---- Tint (final modulation)
                color.rgb *= _TintColor.rgb;

                return color;
            }

            ENDHLSL
        }
    }
}