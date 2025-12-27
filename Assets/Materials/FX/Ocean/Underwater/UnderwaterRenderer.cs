using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

// A modified BLit SRP from Unity's samples for Unity 6.3 LTS
namespace SubnauticaClone
{
    public class UnderwaterRendererFeature : ScriptableRendererFeature
    {
        public Material underwaterMaterial;
        private WaterMaskPass m_Pass;

        public override void Create()
        {
            m_Pass = new WaterMaskPass(underwaterMaterial);
            m_Pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (underwaterMaterial == null)
            {
                Debug.LogWarning("Missing Material!");
                return;
            }

            renderer.EnqueuePass(m_Pass);
        }

        protected override void Dispose(bool disposing)
        {
            m_Pass = null;
        }
    }

    public class WaterMaskPass : ScriptableRenderPass
    {
        private const string m_PassName = "WaterMaskPass";
        private Material m_BlitMaterial;

        public WaterMaskPass(Material mat)
        {
            m_BlitMaterial = mat;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            // UniversalResourceData contains all the texture handles used by the renderer, including the active color and depth textures.
            // The active color and depth textures are the main color and depth buffers that the camera renders into.
            var resourceData = frameData.Get<UniversalResourceData>();

            TextureHandle source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{m_PassName}";
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters param = new(source, destination, m_BlitMaterial, 0);
            renderGraph.AddBlitPass(param, passName: m_PassName);
            resourceData.cameraColor = destination;
        }
    }
}