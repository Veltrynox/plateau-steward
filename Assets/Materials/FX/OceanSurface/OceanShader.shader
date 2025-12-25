Shader "Custom/URPTessellatedOcean"
{
    Properties
    {
        [Header(Base Color)]
        _ColorShallow ("Color Shallow", Color) = (0.3, 0.7, 1, 0.8)
        _ColorDeep ("Color Deep", Color) = (0.0, 0.2, 0.5, 1)
        _DepthFactor ("Depth Factor", Range(0.01, 5)) = 1.0
        _RefractionStrength ("Refraction Strength", Range(0, 0.2)) = 0.02
        _FresnelPower ("Fresnel Power", Range(0.5, 20)) = 5.0

        [Header(PBR)]
        _Smoothness ("Smoothness", Range(0,1)) = 0.95

        [Header(Glitter)]
        _GlitterColor ("Glitter Color", Color) = (1,1,1,1)
        _GlitterThreshold ("Glitter Threshold", Range(0.8, 0.999)) = 0.98
        _GlitterIntensity ("Glitter Intensity", Range(0, 50)) = 10.0
        _GlitterScale ("Glitter Scale", Float) = 500

        [Header(Tessellation)]
        _Tess ("Tessellation Factor", Range(1, 64)) = 16
        _TessMinDist ("Min Distance", Float) = 10
        _TessMaxDist ("Max Distance", Float) = 100
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Off 

            HLSLPROGRAM
            // ---------------------------------------------------------
            // INCLUDES & PRAGMAS
            // ---------------------------------------------------------
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"

            #pragma target 5.0
            #pragma vertex TessellationVertexProgram
            #pragma hull HullProgram
            #pragma domain DomainProgram
            #pragma fragment FragmentProgram

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            // ---------------------------------------------------------
            // VARIABLES & STRUCTS
            // ---------------------------------------------------------
            CBUFFER_START(UnityPerMaterial)
                float4 _ColorShallow;
                float4 _ColorDeep;
                float _DepthFactor;
                float _RefractionStrength;
                float _FresnelPower;
                float _Smoothness;
                float4 _GlitterColor;
                float _GlitterThreshold;
                float _GlitterIntensity;
                float _GlitterScale;
                float _Tess;
                float _TessMinDist;
                float _TessMaxDist;
            CBUFFER_END

            sampler2D _WaterDisplacementMap;
            sampler2D _WaterNormalMap;
            float4 _WaterPatchData; 

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct ControlPoint
            {
                float4 vertex : INTERNALTESSPOS;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
                float3 posWS : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
            };

            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            // ---------------------------------------------------------
            // HELPER FUNCTIONS
            // ---------------------------------------------------------
            float SparkleNoise(float2 uv, float threshold)
            {
                float2 gridID = floor(uv);
                float hashVal = frac(sin(dot(gridID, float2(12.9898, 78.233))) * 43758.5453);
                return step(threshold, hashVal);
            }

            // ---------------------------------------------------------
            // VERTEX STAGE
            // ---------------------------------------------------------
            ControlPoint TessellationVertexProgram(Attributes v)
            {
                ControlPoint p;
                p.vertex = v.vertex;
                p.normal = v.normal;
                p.uv = v.uv;
                return p;
            }

            // ---------------------------------------------------------
            // HULL STAGE
            // ---------------------------------------------------------
            TessellationFactors PatchConstantFunction(InputPatch<ControlPoint, 3> patch)
            {
                float3 p0 = TransformObjectToWorld(patch[0].vertex.xyz);
                float3 p1 = TransformObjectToWorld(patch[1].vertex.xyz);
                float3 p2 = TransformObjectToWorld(patch[2].vertex.xyz);

                float3 tessFactors = GetDistanceBasedTessFactor(p0, p1, p2, _WorldSpaceCameraPos, _TessMinDist, _TessMaxDist);
                tessFactors *= _Tess;

                float4 factors = CalcTriTessFactorsFromEdgeTessFactors(tessFactors);

                TessellationFactors f;
                f.edge[0] = factors.x;
                f.edge[1] = factors.y;
                f.edge[2] = factors.z;
                f.inside = factors.w;
                return f;
            }

            [domain("tri")]
            [partitioning("fractional_odd")]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("PatchConstantFunction")]
            [outputcontrolpoints(3)]
            ControlPoint HullProgram(InputPatch<ControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            // ---------------------------------------------------------
            // DOMAIN STAGE
            // ---------------------------------------------------------
            [domain("tri")]
            void DomainProgram(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 bary : SV_DomainLocation, out Varyings output)
            {
                float3 vertexPosition = patch[0].vertex.xyz * bary.x + patch[1].vertex.xyz * bary.y + patch[2].vertex.xyz * bary.z;
                float2 vertexUV = patch[0].uv * bary.x + patch[1].uv * bary.y + patch[2].uv * bary.z;

                float3 worldPos = TransformObjectToWorld(vertexPosition);

                // Flatten Logic
                float dist = distance(_WorldSpaceCameraPos, worldPos);
                float flattenFade = 1.0 - smoothstep(_TessMaxDist * 0.8, _TessMaxDist, dist);

                // Displacement Logic
                float2 patchCenter = _WaterPatchData.yz;
                float patchSize = _WaterPatchData.x;
                float2 simUV = (worldPos.xz - patchCenter) / patchSize + 0.5;

                float3 displacement = tex2Dlod(_WaterDisplacementMap, float4(simUV, 0, 0)).xyz;
                float3 worldNormalData = tex2Dlod(_WaterNormalMap, float4(simUV, 0, 0)).xyz;

                worldPos += displacement * flattenFade;
                float3 finalNormal = lerp(float3(0,1,0), worldNormalData, flattenFade);

                output.vertex = TransformWorldToHClip(worldPos);
                output.posWS = worldPos;
                output.normalWS = finalNormal; 
                output.viewDirWS = GetWorldSpaceViewDir(worldPos);
                output.uv = vertexUV;
                output.screenPos = ComputeScreenPos(output.vertex);
            }

            // ---------------------------------------------------------
            // FRAGMENT STAGE
            // ---------------------------------------------------------
            half4 FragmentProgram(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                // --- SETUP ---
                float3 viewDir = normalize(input.viewDirWS);
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                float3 normal = normalize(input.normalWS);
                if (!isFrontFace) normal = -normal;

                float4 shadowCoord = TransformWorldToShadowCoord(input.posWS);
                Light mainLight = GetMainLight(shadowCoord);
                half3 bakedGI = SampleSH(normal);
                half occlusion = 1.0;

                float NdotV = saturate(dot(normal, viewDir));
                float fresnel = pow(1.0 - NdotV, _FresnelPower);

                float2 distortion = normal.xz * _RefractionStrength;
                float3 background = SampleSceneColor(screenUV + distortion);

                // --- FOG CALCULATION ---
                float waterFog = 0.0;
                
                if (isFrontFace)
                {
                    float rawDepth = SampleSceneDepth(screenUV);
                    float sceneDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                    float surfaceDepth = LinearEyeDepth(input.screenPos.z / input.screenPos.w, _ZBufferParams);
                    waterFog = 1.0 - exp(-(sceneDepth - surfaceDepth) * _DepthFactor);
                }
                else
                {
                    float distToSurface = distance(_WorldSpaceCameraPos, input.posWS);
                    waterFog = 1.0 - exp(-distToSurface * _DepthFactor * 0.5);
                }
                waterFog = saturate(waterFog);

                float3 albedo = lerp(_ColorShallow.rgb, _ColorDeep.rgb, waterFog);
                float3 refractionColor = lerp(background, albedo, waterFog);
                
                // --- LIGHTING & COMPOSITION ---
                float alpha = 1.0;
                BRDFData brdfData;
                InitializeBRDFData(albedo, 0.0, float3(0,0,0), _Smoothness, alpha, brdfData);

                float3 color = float3(0,0,0);
                
                if (isFrontFace)
                {
                    // Surface
                    float3 GI = GlobalIllumination(brdfData, bakedGI, occlusion, input.posWS, normal, viewDir);
                    GI += LightingPhysicallyBased(brdfData, mainLight, normal, viewDir);
                    color = refractionColor + GI * 0.5;

                    // Glitter
                    float3 H = normalize(mainLight.direction + viewDir);
                    float NdotH = saturate(dot(normal, H));
                    float sunPath = pow(NdotH, 20.0); 
                    float s1 = SparkleNoise(input.posWS.xz * _GlitterScale, _GlitterThreshold);
                    
                    float finalGlitter = (s1) * (sunPath * 5.0 + 0.1) * _GlitterIntensity;
                    finalGlitter *= mainLight.shadowAttenuation * waterFog * pow(fresnel, 20);
                    color += finalGlitter;
                }
                else
                {
                    // Underwater
                    float3 reflectionColor = _ColorDeep.rgb * 0.5;
                    float3 transmission = refractionColor;
                    
                    float3 transmissionColor = GlobalIllumination(brdfData, bakedGI, occlusion, input.posWS, normal, viewDir);
                    transmissionColor += LightingPhysicallyBased(brdfData, mainLight, normal, viewDir);
                    transmission += mainLight.color * mainLight.shadowAttenuation * albedo * 0.2;

                    color = lerp(transmission, reflectionColor, fresnel);
                }
                
                // --- FINAL OUTPUT ---
                color = MixFog(color, ComputeFogFactor(input.posWS.z));
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}