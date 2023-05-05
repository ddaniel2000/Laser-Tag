using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private int _maxBounces = 1;
    [SerializeField]
    private Transform _startPosition;
    [SerializeField]
    private bool _reflectOnlyMirror;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private GameObject _hitMarker;
    private List<GameObject> _hitPointMarkers = new List<GameObject>();
    private Color _startColor = new Color(1, 1, 1, 1);
    private Color _defaultColor = new Color(1, 1, 1, 1);
    private float _time;

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    public static event Action onWinLevel;

    #region Lifecycle

    void Start()
    {

        /* 
         * Instantiate Sphere object and add Line renderer component
         *  Sphere is used to see the Hit Position of the raycast        
        */
        for (int i = 0; i < _maxBounces+1;i++)
        {

            GameObject newObj = Instantiate(_hitMarker, transform);
            _hitPointMarkers.Add(newObj);
            newObj.AddComponent<LineRenderer>();
            LineRenderer _lr = newObj.GetComponent<LineRenderer>();
            _lineRenderers.Add(_lr);
            _lr.material = _material;
            _lr.startColor = _startColor;
            _lr.endColor = _startColor;

        }
        
    }

    // Update is called once per frame
    void Update()
    {

        CastLaser(this._startPosition.position, -this._startPosition.forward);

        CheckGlassTouched(_time);

    }

    #endregion

    #region Private

    /// <summary>
    /// Cast Laser 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    private void CastLaser(Vector3 position, Vector3 direction)
    {

        // Set starting Sphere and LineRenderer's position[0], at the Start Position
        _hitPointMarkers[0].transform.position = this.transform.position;
        _lineRenderers[0].SetPosition(0, this.transform.position);

        for (int i= 0; i < _maxBounces; i++)
        {
            
            Ray ray = new Ray(position, direction);

            RaycastHit hit;

            
            if (Physics.Raycast( ray, out hit, float.PositiveInfinity, 1))
            {
 
                position = hit.point;

                _hitPointMarkers[i+1].transform.position = hit.point;
                _lineRenderers[i].SetPosition(1, _hitPointMarkers[i+1].transform.position);
                _lineRenderers[i+1].SetPosition(0, hit.point);

                // Set the last LineRenderer's position to the last hit
                _lineRenderers[_maxBounces].SetPosition(1, _hitPointMarkers[_maxBounces].transform.position);
                _lineRenderers[_maxBounces].SetPosition(0, _hitPointMarkers[_maxBounces].transform.position);

                if (hit.transform.tag == "Glass" )
                {

                    // if _time == Time.deltaTime -> Glass is touched
                    _time = Time.deltaTime;
                    position = hit.point + direction;
                    //Get the color of "Glass" object
                    GetColor(hit, i);                

                }
                else
                {

                    direction = Vector3.Reflect(direction, hit.normal);

                }

                if (hit.transform.tag == "Win" )
                {
                                       
                    position = hit.point;
                    _hitPointMarkers[i + 1].transform.position = hit.point;
                    _lineRenderers[i].SetPosition(1, _hitPointMarkers[i + 1].transform.position);
                    _lineRenderers[i + 1].SetPosition(0, hit.point);

                    // Get the color of "Win" object
                    GetColor(hit, i);

                }

                if (hit.transform.tag == "Enviroment"  && _reflectOnlyMirror)
                {

                    for (int j = (i + 1); j <= _maxBounces; j++)
                    {

                        position = hit.point;
                        _hitPointMarkers[j].transform.position = hit.point;
                        _lineRenderers[j-1].SetPosition(1, _hitPointMarkers[j - 1].transform.position);
                        _lineRenderers[j - 1].SetPosition(0, hit.point);
                       
                    }
                    break;

                }              
            }
        }
    }

    /// <summary>
    /// Get the color of Hit object
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="index"></param>
    private void GetColor(RaycastHit hit, int index)
    {

        Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
       
        if (renderer != null)
        {
            Color hitColor = renderer.material.color;
            

            if (hit.transform.tag != "Win")
            {

                if(index == 0)
                {

                    _startColor = _defaultColor;

                }
                if(_startColor == Color.white)
                {

                    _startColor = hitColor;

                }
                else
                {

                    _startColor = LerpViaHSB(_startColor, hitColor, 0.5f);

                }
                

                //stiu ca e HardCoded dar nu imi dadea valorile RGB corecte, erau mereu variatii de ~0.2

                
                if (_startColor.r < 0.39f && _startColor.g == 1 && _startColor.b < 0.009f )
                {
                    _startColor = Color.green;
                }
                else if (_startColor.r == 1 && _startColor.g > 0.09f && _startColor.g < 0.32f && _startColor.b > 0f && _startColor.b < 0.1f)
                {
                    _startColor = Color.red;
                }
                else if (_startColor.b == 1f && _startColor.r == 0f && _startColor.g > 0.2f && _startColor.g < 0.4f)
                {
                    _startColor = Color.blue;
                }

                
                for (int x = index; x < _maxBounces; x++)
                {

                    _lineRenderers[x + 1].startColor = _startColor;
                    _lineRenderers[x + 1].endColor = _startColor;

                }
            }
            else
            {

                CompareColorsToWin(hitColor, _startColor);

            }            
        }
    }

    /// <summary>
    /// Compare the color of the LineRenderer with "Win" object 
    /// </summary>
    /// <param name="winColor"></param>
    /// <param name="lineColor"></param>
    private void CompareColorsToWin(Color winColor, Color lineColor)
    {
        
        string winColorString = winColor.ToString();
        string lineColorString = lineColor.ToString();
       
        if (winColor == lineColor)
        {
            
            // Start WinLevel process
            onWinLevel?.Invoke();

        }

    }


    /// <summary>
    /// Reset the color of the Line Renderer 
    /// </summary>
    private void ResetColor()
    {
       
        for (int i=0; i <= _maxBounces; i++)
        {
            
            _lineRenderers[i].startColor = _defaultColor;
            _lineRenderers[i].endColor = _defaultColor;
            
        }

    }

    /// <summary>
    /// Check if the "Glass" object is touched
    /// </summary>
    /// <param name="time"></param>
    private void CheckGlassTouched(float time)
    {

        // if _time is different than Time.deltaTime , it means "Glass" is not touched anymore
        if (time != Time.deltaTime)
        {
           
            ResetColor();

        }

    }
    #endregion

    #region ColorCobinerRGBtoHSB

    /// <summary>
    /// Color Combiner from RGB to HSB
    /// </summary>

    [System.Serializable]
    private struct HSBColor
    {
        public float h;
        public float s;
        public float b;
        public float a;

        public HSBColor(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }

        public static HSBColor FromColor(Color color)
        {
            HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);

            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
            {
                return ret;
            }

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min)
            {
                if (g == max)
                {
                    ret.h = (b - r) / dif * 60f + 120f;
                }
                else if (b == max)
                {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.h = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.h = (g - b) / dif * 60f;
                }
                if (ret.h < 0)
                {
                    ret.h = ret.h + 360f;
                }
            }
            else
            {
                ret.h = 0;
            }

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;

            return ret;
        }

        public static Color ToColor(HSBColor hsbColor)
        {
            float r = hsbColor.b;
            float g = hsbColor.b;
            float b = hsbColor.b;
            if (hsbColor.s != 0)
            {
                float max = hsbColor.b;
                float dif = hsbColor.b * hsbColor.s;
                float min = hsbColor.b - dif;

                float h = hsbColor.h * 360f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
        {
            float h, s;

            if (a.b == 0)
            {
                h = b.h;
                s = b.s;
            }
            else if (b.b == 0)
            {
                h = a.h;
                s = a.s;
            }
            else
            {
                if (a.s == 0)
                {
                    h = b.h;
                }
                else if (b.s == 0)
                {
                    h = a.h;
                }
                else
                {
                    // works around bug with LerpAngle
                    float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                    while (angle < 0f)
                        angle += 360f;
                    while (angle > 360f)
                        angle -= 360f;
                    h = angle / 360f;
                }
                s = Mathf.Lerp(a.s, b.s, t);
            }
            return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }

    }
    private Color LerpViaHSB(Color a, Color b, float t)
    {
        return HSBColor.Lerp(HSBColor.FromColor(a), HSBColor.FromColor(b), t).ToColor();
    }

    #endregion

}
