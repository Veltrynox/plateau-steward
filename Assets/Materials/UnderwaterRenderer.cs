using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule.Util;


public class UnderwaterRenderer : ScriptableRenderPass
{
    const string m_PassName = "UnderwaterRenderer";
    
    Material m_BlitMaterial;

    // Function used to transfer the material from the renderer feature to the render pass.
    public void Setup(Material mat)
    {
        m_BlitMaterial = mat;
        requiresIntermediateTexture = true;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        // UniversalResourceData contains all the texture handles used by the renderer, including the active color and depth textures
        // The active color and depth textures are the main color and depth buffers that the camera renders into
        var resourceData = frameData.Get<UniversalResourceData>();

        // The destination texture is created here,
        // the texture is created with the same dimensions as the active color texture
        var colorSrc = resourceData.activeColorTexture;
        var depthSrc = resourceData.activeDepthTexture;

        var destinationDesc = renderGraph.GetTextureDesc(colorSrc);
        destinationDesc.name = $"CameraColor-{m_PassName}";
        destinationDesc.clearBuffer = false;

        TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

        // m_BlitMaterial.SetTexture("_CameraDepthTexture", depthSrc);

        RenderGraphUtils.BlitMaterialParameters parameter = new(colorSrc, destination, m_BlitMaterial, 0);
        renderGraph.AddBlitPass(parameter, passName: m_PassName);

        //FrameData allows to get and set internal pipeline buffers. Here we update the CameraColorBuffer to the texture that we just wrote to in this pass. 
        //Because RenderGraph manages the pipeline resources and dependencies, following up passes will correctly use the right color buffer.
        //This optimization has some caveats. You have to be careful when the color buffer is persistent across frames and between different cameras, such as in camera stacking.
        //In those cases you need to make sure your texture is an RTHandle and that you properly manage the lifecycle of it.
        resourceData.cameraColor = destination;
    }
}

public class UnderwaterRendererRendererFeature : ScriptableRendererFeature
{
    [Tooltip("The material used when making the blit operation.")]
    public Material material;

    [Tooltip("The event where to inject the pass.")]
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

    UnderwaterRenderer m_Pass;

    // Here you can create passes and do the initialization of them. This is called everytime serialization happens.
    public override void Create()
    {
        m_Pass = new UnderwaterRenderer();

        // Configures where the render pass should be injected.
        m_Pass.renderPassEvent = renderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Early exit if there are no materials.
        if (material == null)
        {
            Debug.LogWarning(this.name + " material is null and will be skipped.");
            return;
        }

        m_Pass.Setup(material);
        renderer.EnqueuePass(m_Pass);
    }
}


