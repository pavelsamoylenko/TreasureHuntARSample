using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

namespace xSite
{
    public class MaterialTextureReplacement : MonoBehaviour
    {
        [SerializeField] private Material matToChange;
        [SerializeField] private CPUImageExtractor cpuImageExtractor;

        private void OnEnable()
        {
            cpuImageExtractor.OnCpuImageExtracted += OnTextureReceived;
        }
        private void OnDisable()
        {
            cpuImageExtractor.OnCpuImageExtracted -= OnTextureReceived;
        }
        

        public void OnTextureReceived(Texture2D texture2D)
        {
            matToChange.mainTexture = texture2D;
        }
        
        
        
    }
}