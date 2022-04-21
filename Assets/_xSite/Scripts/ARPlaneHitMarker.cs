using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace xSite
{
    public class ARPlaneHitMarker : MonoBehaviour
    {
        [SerializeField] private MaterialTextureReplacement _groundRenderer;
        
        private ARModuleAPI _api;
        private Transform _cameraTransform;
        private Vector2 _screenCenter;
        public List<ARRaycastHit> Hits { get; set; }

        public Vector3 LastHitPosition;

        public void Construct(ARModuleAPI arModuleAPI)
        {
            _api = arModuleAPI;
            _cameraTransform = _api.Camera.transform;
        }

        public void Initialize()
        {
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _groundRenderer.Initialize(_api.CPUImageExtractor);
        }
        
        private void Update()
        {
            Hits = new List<ARRaycastHit>();
            _api.RaycastManager.Raycast(_screenCenter, Hits, TrackableType.PlaneWithinPolygon);

            if (Hits.Count > 0)
            {
                
                var position = LastHitPosition = Hits[0].pose.position;
                position += new Vector3(0f, 0.01f, 0f);

                var rotation = _cameraTransform.rotation;
                rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
                transform.position = position;
                transform.rotation = rotation;
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Hide()
        {   
            //gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}