using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleColor : MonoBehaviour
{
    [SerializeField]
    private Color _color;
    private Renderer _meshRenderer;
    private Material _material;

    private void Start()
    {
        _meshRenderer = GetComponent<Renderer>();
        _material = GetComponent<Material>();
        _meshRenderer.material.color = _color;
    }
}
