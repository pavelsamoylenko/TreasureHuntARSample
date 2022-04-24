using UnityEngine;

namespace xSite
{
    public class MaterialTextureReplacement : MonoBehaviour
    {
        [SerializeField] private Renderer materialRenderer;
        
        private CPUImageExtractor _cpuImageExtractor;
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private bool _scaled;


        public void Initialize(CPUImageExtractor cpuImageExtractor)
        {
            _cpuImageExtractor = cpuImageExtractor;
            _cpuImageExtractor.OnCpuImageExtracted += OnTextureReceived;
        }

        private void OnDestroy()
        {
            if(!_cpuImageExtractor) return;
            _cpuImageExtractor.OnCpuImageExtracted -= OnTextureReceived;
        }


        private void OnTextureReceived(Texture2D texture2D)
        {
            Debug.Log("Texture Received");
            if(!_scaled) SetScale();
            materialRenderer.material.SetTexture(BaseMap, texture2D);
            //materialRenderer.sharedMaterial.SetTexture(BaseMap, texture2D);
        }

        private void SetScale()
        {
            var x = Screen.height / Screen.width;
            var transform1 = transform;
            var localScale = transform1.localScale;
            localScale =
                new Vector3(localScale.x, x * localScale.y, localScale.z);
            transform1.localScale = localScale;
            _scaled = true;
        }
    }
}