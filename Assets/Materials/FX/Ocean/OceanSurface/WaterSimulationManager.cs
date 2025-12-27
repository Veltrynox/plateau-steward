using UnityEngine;

public class WaterSimulationManager : MonoBehaviour
{
    [Header("Compute Shader")]
    public ComputeShader waveComputeShader;
    private int kernelHandle;

    [Header("Simulation Settings")]
    public int resolution = 512;
    public float patchSize = 200f;
    public WaveParameter[] waves;

    private RenderTexture displacementTexture;
    private RenderTexture normalTexture;
    private ComputeBuffer waveBuffer;

    private static readonly int _WaterDisplacementMap = Shader.PropertyToID("_WaterDisplacementMap");
    private static readonly int _WaterNormalMap = Shader.PropertyToID("_WaterNormalMap");
    private static readonly int _WaterPatchData = Shader.PropertyToID("_WaterPatchData");

    [System.Serializable]
    public struct WaveParameter
    {
        [Range(0, 1)] public float steepness;
        public float wavelength;
        public float speed;
        [Range(-360, 360)] public float directionAngle;
    };

    void Start()
    {
        // clipping fix
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null) mf.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

        InitData();
    }

    void InitData()
    {
        kernelHandle = waveComputeShader.FindKernel("CSMain");

        // Textures
        if (displacementTexture != null) displacementTexture.Release();
        displacementTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBHalf);
        displacementTexture.enableRandomWrite = true;
        displacementTexture.name = "WaterDisplacementTex";
        displacementTexture.Create();
        displacementTexture.wrapMode = TextureWrapMode.Repeat;

        if (normalTexture != null) normalTexture.Release();
        normalTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBHalf);
        normalTexture.enableRandomWrite = true;
        normalTexture.name = "WaterNormalTex";
        normalTexture.Create();
        normalTexture.wrapMode = TextureWrapMode.Repeat;

        // Buffers
        if (waveBuffer != null) waveBuffer.Release();
        waveBuffer = new ComputeBuffer(waves.Length != 0 ? waves.Length : 1, 16);
    }

    void Update()
    {
        if (displacementTexture == null || !displacementTexture.IsCreated()) InitData();

        Vector3 centerPos = Camera.main.transform.position;

        // Wave Data
        if (waves.Length > 0)
        {
            Vector4[] rawWaveData = new Vector4[waves.Length];
            for (int i = 0; i < waves.Length; ++i)
            {
                rawWaveData[i] = new Vector4(waves[i].steepness, waves[i].wavelength, waves[i].speed, waves[i].directionAngle);
            }
            waveBuffer.SetData(rawWaveData);
            waveComputeShader.SetInt("_WaveCount", waves.Length);
        }
        else
        {
            waveComputeShader.SetInt("_WaveCount", 0);
        }

        // Dispatch
        waveComputeShader.SetBuffer(kernelHandle, "_WaveDataBuffer", waveBuffer);
        waveComputeShader.SetFloat("_Time", Time.time);
        waveComputeShader.SetFloat("_PatchSize", patchSize);
        waveComputeShader.SetVector("_PatchCenter", centerPos);
        waveComputeShader.SetInt("_Resolution", resolution);

        waveComputeShader.SetTexture(kernelHandle, "Result", displacementTexture);
        waveComputeShader.SetTexture(kernelHandle, "ResultNormal", normalTexture);
        
        int threadGroups = Mathf.CeilToInt(resolution / 8.0f);
        waveComputeShader.Dispatch(kernelHandle, threadGroups, threadGroups, 1);

        // Global Uniforms
        Shader.SetGlobalTexture(_WaterDisplacementMap, displacementTexture);
        Shader.SetGlobalTexture(_WaterNormalMap, normalTexture);
        Shader.SetGlobalVector(_WaterPatchData, new Vector4(patchSize, centerPos.x, centerPos.z, 0));
    }

    void OnDestroy()
    {
        if (waveBuffer != null) waveBuffer.Release();
        if (displacementTexture != null) displacementTexture.Release();
        if (normalTexture != null) normalTexture.Release();
    }
}