using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace xSite
{
    public class CPUImageExtractor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        private Texture2D m_CameraTexture;

        private XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.MirrorX |
                                                             XRCpuImage.Transformation.MirrorY;


        /// <summary>
        /// Cycles the image transformation to the next case.
        /// </summary>

        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }


        public event Action<Texture2D> OnCpuImageExtracted;

        void OnEnable()
        {
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived += OnCameraFrameReceived;
            }
        }

        void OnDisable()
        {
            if (m_CameraManager != null)
            {
                m_CameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }

        private unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs obj)
        {
            if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                Debug.Log("haven't got cpuimage");
                return;
            }
            
            Debug.Log("Got a CPU image");
            var format = TextureFormat.RGBA32;

            if (m_CameraTexture == null ||
                m_CameraTexture.width != image.width ||
                m_CameraTexture.height != image.height)
            {
                m_CameraTexture = new Texture2D(image.width, image.height, format, false);
            }
            
            // Convert the image to format, flipping the image across the Y axis.
            // We can also get a sub rectangle, but we'll get the full image here.
            var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

            // Texture2D allows us write directly to the raw texture data
            // This allows us to do the conversion in-place without making any copies.
            var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
            try
            {
                image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
            finally
            {
                // We must dispose of the XRCpuImage after we're finished
                // with it to avoid leaking native resources.
                image.Dispose();
            }

            // Apply the updated texture data to our texture
            m_CameraTexture.Apply();

            rawTextureData.Dispose();
            
            OnCpuImageExtracted?.Invoke(m_CameraTexture);
        }
    }
}