#ifndef WATER_COMMON_INCLUDED
#define WATER_COMMON_INCLUDED

TEXTURE2D(_WaterDisplacementMap); 
SAMPLER(sampler_WaterDisplacementMap);
TEXTURE2D(_WaterNormalMap); 
SAMPLER(sampler_WaterNormalMap);

float4 _WaterPatchData;

// Shared function to get water data at a specific World Position
void GetWaterSurfaceData(float3 worldPos, float lod, out float3 displacement, out float3 normal)
{
    float2 patchCenter = _WaterPatchData.yz;
    float patchSize = _WaterPatchData.x;
    
    float2 uv = (worldPos.xz - patchCenter) / patchSize + 0.5;
    
    displacement = SAMPLE_TEXTURE2D(_WaterDisplacementMap, sampler_WaterDisplacementMap, uv).xyz;
    normal = SAMPLE_TEXTURE2D(_WaterNormalMap, sampler_WaterNormalMap, uv).xyz;
}

#endif