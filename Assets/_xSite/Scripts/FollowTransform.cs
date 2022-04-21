using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform toFollow;
    [SerializeField] private float lerp = 0.9f;
    [SerializeField] private Vector3 _offset = new Vector3(0f,0.5f,0f);

    private Quaternion _rotationOffset;
    
    private bool _isInitialized;

    public Vector3 offset
    {
        get => _offset;
        set => _offset = value;
    }


    private void Start()
    {
        if (toFollow)
            _isInitialized = true;
    }

    public void Initialize(Transform transformToFollow, float lerpAmount = 0.9f)
    {
        toFollow = transformToFollow;
        lerp = lerpAmount;
    }
    private void LateUpdate()
    {
        if(!_isInitialized || toFollow == null) return;
        transform.position = Vector3.Lerp(transform.position, toFollow.position + offset, lerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, toFollow.rotation, lerp);
    }
}
