using System;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Mathematics;
using UnityEngine;
using xSite;

public class ShovelController : MonoBehaviour
{
    public GameObject ShovelGO;
    public Ease Ease = Ease.InElastic;

    [SerializeField] private ARModuleAPI arModuleAPI;
    [SerializeField] private FollowTransform followTransform;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private AudioSource audioSource;

    [Space]
    [SerializeField] private MaterialTextureReplacement treasurePrefab;

    [Space]
    [SerializeField] private GameObject depthQuadPrefab;
    

    private ARPlaneHitMarker _planeHitMarker;

    private MeshRenderer[] _shovelRenderers;
    private TweenerCore<Color, Color, ColorOptions> _fadeTween;
    private TweenerCore<Vector3, Vector3, VectorOptions> _failAnimationTween;
    private GameObject _spawnedDepthQuad;
    private MaterialTextureReplacement _spawnedTreasure;
    private bool _spawning;


    private void Awake()
    {
        _planeHitMarker = arModuleAPI.PlaneHitMarker;
        _shovelRenderers ??= GetComponentsInChildren<MeshRenderer>();
        followTransform ??= GetComponent<FollowTransform>();
        audioSource ??= GetComponent<AudioSource>();

    }

    private void OnEnable()
    {
        ShovelGO.SetActive(true);
        followTransform.Initialize(arModuleAPI.PlaneHitMarker.transform);
        followTransform.enabled = true;
        _spawnedDepthQuad = Instantiate(depthQuadPrefab, arModuleAPI.PlaneHitMarker.transform);
        _spawnedDepthQuad.transform.localPosition = Vector3.zero;
        _spawnedDepthQuad.SetActive(false);
    }

    private void OnDisable()
    {
        ShovelGO.SetActive(false);
        followTransform.enabled = false;
        _spawnedDepthQuad.SetActive(false);
    }

    private void Update()
    {
        Show();
    }

    private void Show()
    {
        if(_spawning) return;
        var hits = _planeHitMarker.Hits;
        if (hits.Count > 0)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    public async void OnSuccess()
    {
        _spawning = true;
        if(_spawnedTreasure != null) Destroy(_spawnedTreasure.gameObject);

        gameObject.SetActive(false);
        arModuleAPI.PlaneHitMarker.Hide();
        
        var hitPosition = _planeHitMarker.transform.position;

        _spawnedTreasure = Instantiate(treasurePrefab, hitPosition, Quaternion.identity, arModuleAPI.ARSessionOrigin.transform);
        _spawnedTreasure.transform.LookAt(arModuleAPI.Camera.transform, Vector3.up);
        _spawnedTreasure.transform.localRotation = Quaternion.Euler(0f, _spawnedTreasure.transform.rotation.y + 180f, 0f);
        _spawnedTreasure.gameObject.SetActive(true);

        await Task.Delay(new TimeSpan(0, 0, 15));
        arModuleAPI.PlaneHitMarker.Show();
        gameObject.SetActive(true);
        _spawning = false;
        Destroy(_spawnedTreasure.gameObject);
    }

    public void ShowFailAnimation()
    {
        var initialPos = transform.position;
        var hitPosition = _planeHitMarker.transform.position;
        followTransform.enabled = false;
        _planeHitMarker.SetMarker(false);

        _spawnedDepthQuad.transform.position = hitPosition;
        _spawnedDepthQuad.SetActive(true);
        audioSource.Play();


        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(hitPosition, animationDuration)
            .SetEase(Ease));
        sequence.Append(transform.DOMove(initialPos, animationDuration).SetEase(Ease.Linear));
        sequence.OnComplete(() =>
        {
            followTransform.enabled = true;
            _planeHitMarker.SetMarker(true);
            _spawnedDepthQuad.SetActive(false);
        });

        sequence.Play();
    }

    [ContextMenu("FadeIn")]
    public void FadeIn()
    {
        foreach (var shovelRenderer in _shovelRenderers)
        {
            var material = shovelRenderer.material;
            _fadeTween = material.DOFade(1f, 0.3f);
        }
    }

    [ContextMenu("FadeOut")]
    public void FadeOut()
    {
        foreach (var shovelRenderer in _shovelRenderers)
        {
            var material = shovelRenderer.material;
            _fadeTween = material.DOFade(0.5f, 0.3f);
        }
    }
}