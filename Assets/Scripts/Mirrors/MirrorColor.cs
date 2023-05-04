using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorColor : MonoBehaviour
{
    
    private Renderer _meshRenderer;
    private Material _material;
    private Color _color;
    [SerializeField]
    private string _colorName;

    #region Lifecycle

    void Start()
    {

        _meshRenderer = GetComponent<Renderer>();
        _material = GetComponent<Material>();     
        _color = (Color)typeof(Color).GetProperty(_colorName.ToLowerInvariant()).GetValue(null, null);
        _meshRenderer.material.color = _color;

    }

    #endregion

}
