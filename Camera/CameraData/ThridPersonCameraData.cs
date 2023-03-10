using UnityEngine;

[CreateAssetMenu(fileName = "ThirdPersonCamera", menuName = "ScriptableObjects/ThirdPersonCamera")]
public class ThridPersonCameraData : ScriptableObject
{
    [Header("Camera")]
    [Range(40, 100)] public float _FOV;
    [Space]

    [Range(0.03f, 0.2f)] public float _cameraFollowSpeed;
    [Space]

    [Range(0.1f, 110f)] public float _cameraLookSpeed;
    [Range(0.07f, 110f)] public float _cameraPivotSpeed;
    [Space]

    [Range(-80, 0)] public float _cameraPivotMin;
    [Range(0, 80)] public float _cameraPivotMax;
    [Space]

    public bool _invertedLook;
    public bool _invertadPivot;
    [Space]

    [Header("Camera Collision")]
    public float _collisionRange;
    public float _collisionOffset;
    public LayerMask _collisionLayers;
}
