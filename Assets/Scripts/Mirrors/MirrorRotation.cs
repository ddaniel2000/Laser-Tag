using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MirrorRotation : MonoBehaviour
{
    private float _sensitivity = 200;

    private void OnEnable()
    {
        Sensitvity.OnSliderValueChanged += ChangeSensitivity;
    }

    private void OnDisable()
    {
        Sensitvity.OnSliderValueChanged -= ChangeSensitivity;
    }

    private void Start()
    {

    }
    private void OnMouseDrag()
    {
        Rotate();

    }

    private void Rotate()
    {
       
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime, Space.Self);
        Debug.Log(_sensitivity);
    }

    private void ChangeSensitivity(float value)
    {
       
        _sensitivity = value * 100;
    }
}
