using Highlighters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : GenericSingleton<HighlightManager>
{
    [SerializeField] private InteractableObjectsHighlight _highlighterSettings;
    private List<GameObject> _outlineRenderers = new List<GameObject>();
    private List<GameObject> _highlightRenderers = new List<GameObject>();
    [SerializeField] private Highlighter _outlineHighlighter;
    [SerializeField] private Highlighter _highlighter;

    //[SerializeField] private Highlighter[] tempArray;

    // [ContextMenu("Find Interactabeles")]
    // public void GetHighlighters()
    // {
    //     tempArray = FindObjectsOfType<Highlighter>();
    // }
    //
    // [ContextMenu("Delete Highlighters")]
    // public void DestroyHighlighters()
    // {
    //     foreach (var item in tempArray)
    //     {
    //         DestroyImmediate(item.GetComponent<Highlighter>());
    //     }
    // }

    private void Awake()
    {
        StartCoroutine(Highlighting());
    }

    public void AddToOutline(GameObject renderer)
    {
        if (_outlineRenderers.Contains(renderer))
            return;

        _outlineRenderers.Add(renderer);
        UpdateOutlineRenderers();
    }
    public void AddToHighlight(GameObject renderer)
    {
        if (_highlightRenderers.Contains(renderer))
            return;

        _highlightRenderers.Add(renderer);
        UpdateHighlightRenderers();
    }

    public void RemoveFromOutline(GameObject renderer)
    {
        if (!_outlineRenderers.Contains(renderer))
            return;

        _outlineRenderers.Remove(renderer);
        UpdateOutlineRenderers();
    }
    public void RemoveFromHighlight(GameObject renderer)
    {
        if (!_highlightRenderers.Contains(renderer))
            return;

        _highlightRenderers.Remove(renderer);
        UpdateHighlightRenderers();
    }

    private void UpdateOutlineRenderers()
    {
        _outlineHighlighter.Renderers.Clear();
        foreach (var item in _outlineRenderers)
        {
            var renderers = item.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                _outlineHighlighter.Renderers.Add(new HighlighterRenderer(renderer, renderer.sharedMaterials.Length));
            }   
        }
    }

    private void UpdateHighlightRenderers()
    {
        _highlighter.Renderers.Clear();
        foreach (var item in _highlightRenderers)
        {
            var renderers = item.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                _highlighter.Renderers.Add(new HighlighterRenderer(renderer, renderer.sharedMaterials.Length));
            }
        }
    }

    protected IEnumerator Highlighting()
    {
        float t;
        Color overlayColor;
        float blurInstensity;

        while (true)
        {
            t = Mathf.PingPong(Time.time, 1);

            overlayColor = Color.Lerp(_highlighterSettings.ColorMin, _highlighterSettings.ColorMax, t);
            blurInstensity = Mathf.Lerp(_highlighterSettings.MinBlur, _highlighterSettings.MaxBlur, t);

            _highlighter.Settings.BlurIntensity = blurInstensity;
            _highlighter.Settings.OverlayFront.Color = overlayColor;


            yield return new WaitForEndOfFrame();
        }
    }

}
