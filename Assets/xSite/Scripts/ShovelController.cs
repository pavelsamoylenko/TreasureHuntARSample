using System;
using System.Collections;
using System.Collections.Generic;
using ARFoundationRemote.Runtime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ShovelController : MonoBehaviour
{
    [Header("Put your PlaneMarker here"), SerializeField]
     private GameObject PlaneMarkerPrefab;
    [SerializeField] private Camera ARCamera;

    [FormerlySerializedAs("_planeManager"), SerializeField] 
    private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager ARRaycastManagerScript;
    public GameObject ObjectToSpawn;
    public Ease Ease = Ease.InOutBounce;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    private GameObject SelectedObject;

    private Vector2 TouchPosition;


    private Quaternion YRotation;

    public bool Enabled = false;

    public bool Rotation;

    public bool Recharging;
    

    private void OnEnable()
    {
        // TODO: To planemarker to another class
        PlaneMarkerPrefab.SetActive(true);
    }

    private void OnDisable()
    {
        PlaneMarkerPrefab.SetActive(false);
    }

    void Update()
    {
        if (Enabled)
        {
            ShowMarker();
        }

        MoveAndRotateObject();
    }

    void ShowMarker()
    {
        hits = new List<ARRaycastHit>();

        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);

        if (hits.Count > 0)
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position;
            PlaneMarkerPrefab.SetActive(true);
        }
        
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation);
            Enabled = false;
            PlaneMarkerPrefab.SetActive(false);
        }
    }


    [ContextMenu("Shovel")]
    public void ShowFailAnimation()
    {
        var initialPos = transform.position;
        
        var tw = transform.DOMove(new Vector3(transform.position.x + 0.5f, 0f, transform.position.z + 0.5f), 1f)
            .SetEase(Ease)
            .OnComplete(() => transform.DOMove(initialPos, 1f).SetEase(Ease));
    }

    void MoveAndRotateObject()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPosition = touch.position;
            
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                // Select choosed object
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("UnSelected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }

            SelectedObject = GameObject.FindWithTag("Selected");

            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1 )
            {
                if (Rotation)
                {
                   // Rotate Object by 1 finger
                    YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f);
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation;
                }
                else
                {
                   // Move Object
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                    SelectedObject.transform.position = hits[0].pose.position;
                }
            }
            
            // Deselect object
            if (touch.phase == TouchPhase.Ended)
            {
                if (SelectedObject.CompareTag("Selected"))
                {
                    SelectedObject.tag = "UnSelected";
                }
            }
        }
    }
}
