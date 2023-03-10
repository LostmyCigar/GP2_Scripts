using UnityEngine;

[CreateAssetMenu(fileName = "GhostData", menuName = "ScriptableObjects/PlayerData")]
public class GhostData : ScriptableObject {
    public float MoveSpeed;
    public float RotationSpeed;
    [Tooltip("This value should be above 1 and is only used when the user doesnt give input")]
    public float Deaccel;

    public float InteractionRange;
}
