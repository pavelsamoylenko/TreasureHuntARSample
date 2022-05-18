using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace xSite
{
    public class ARModuleAPI : MonoBehaviour
    {
        [Header("Scene Objects")]
        public Camera Camera;
        public ARCameraManager ARCameraManager;
        public ARSessionOrigin ARSessionOrigin;
        public ARSession ARSession;
        public ARRaycastManager RaycastManager;
        public ARPlaneManager PlaneManager;
        public CPUImageExtractor CPUImageExtractor;
        
        [Space, Header("Prefabs")]
        public ARPlaneHitMarker PlaneHitMarkerPrefab;

        public ARPlaneHitMarker PlaneHitMarker { get; private set; }

        private void Awake()
        {
            SpawnPlaneHitMarker();
        }

        private void SpawnPlaneHitMarker()
        {
            PlaneHitMarker = Instantiate(PlaneHitMarkerPrefab, ARSessionOrigin.transform);
            PlaneHitMarker.Construct(this);
            PlaneHitMarker.Initialize();
        }
    }
}