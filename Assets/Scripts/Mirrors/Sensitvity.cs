using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Sensitvity : MonoBehaviour
{
    public static event Action<float> OnSliderValueChanged;
    private Slider _slider;
    private float _value;

    private void Start()
    {
        _slider = GetComponent<Slider>();

    }
    public void ValueChanged()
    {
        _value = _slider.value;
        OnSliderValueChanged?.Invoke(_value);
    }
}
