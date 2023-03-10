using UnityEngine;

[CreateAssetMenu(fileName = "InteractionHighlightData", menuName = "ScriptableObjects/InteractionHighlightData")]
public class InteractableObjectsHighlight : ScriptableObject
{
    [Header("Outline")]
    public Color OutlineColor;
    public float blurSize = 0.0131f;
    public float MinBlur = 0;
    public float MaxBlur = 1;

    [Header("Highlight")]
    public Color ColorMin;
    public Color ColorMax;
}
