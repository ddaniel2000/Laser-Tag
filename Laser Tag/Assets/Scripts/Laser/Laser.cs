using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private GameObject _sphere;
    private List<GameObject> _sphereArray = new List<GameObject>();

    private Color _startColor = new Color(1, 1, 1, 1);
    private Color _defaultColor = new Color(1, 1, 1, 1);

    private float _time;

    public delegate void OnWinLevel();
    public static OnWinLevel onWinLevel;

    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    #region Lifecycle

    void Start()
    {

        /* 
         * Instantiate Sphere object and add Line renderer component
         *  Sphere is used to see the Hit Position of the raycast        
        */
        for (int i = 0; i < _maxBounces+1;i++)
        {

            GameObject newObj = Instantiate(_sphere, transform);
            _sphereArray.Add(newObj);
            newObj.AddComponent<LineRenderer>();
            LineRenderer _lr = newObj.GetComponent<LineRenderer>();
            _lineRenderers.Add(_lr);
            _lr.material = _material;
            _lr.startColor = _startColor;
            _lr.endColor = _startColor;

        }
        
    }

    // Update is called once per frame
    void LateUpdate()
    {

        CastLaser(this._startPosition.position, -this._startPosition.forward);

        CheckGlassTouched(_time);

    }

    #endregion

    #region Private
    private void CastLaser(Vector3 position, Vector3 direction)
    {

        // Set starting Sphere and LineRenderer's position[0], at the Start Position
        _sphereArray[0].transform.position = this.transform.position;
        _lineRenderers[0].SetPosition(0, this.transform.position);

        for (int i= 0; i < _maxBounces; i++)
        {
            
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            
            if (Physics.Raycast( ray, out hit, 300, 1))
            {
 
                position = hit.point;

                _sphereArray[i+1].transform.position = hit.point;
                _lineRenderers[i].SetPosition(1, _sphereArray[i+1].transform.position);
                _lineRenderers[i+1].SetPosition(0, hit.point);

                // Set the last LineRenderer's position to the last hit
                _lineRenderers[_maxBounces].SetPosition(1, _sphereArray[_maxBounces].transform.position);
                _lineRenderers[_maxBounces].SetPosition(0, _sphereArray[_maxBounces].transform.position);

                if (hit.transform.tag == "Glass" )
                {

                    // if _time == Time.deltaTime -> Glass is touched
                    _time = Time.deltaTime;

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
                    _sphereArray[i + 1].transform.position = hit.point;
                    _lineRenderers[i].SetPosition(1, _sphereArray[i + 1].transform.position);
                    _lineRenderers[i + 1].SetPosition(0, hit.point);

                    // Get the color of "Win" object
                    GetColor(hit, i);

                }

                if (hit.transform.tag == "Enviroment"  && _reflectOnlyMirror)
                {

                    for (int j = (i + 1); j <= _maxBounces; j++)
                    {

                        position = hit.point;
                        _sphereArray[j].transform.position = hit.point;
                        _lineRenderers[j-1].SetPosition(1, _sphereArray[j - 1].transform.position);
                        _lineRenderers[j - 1].SetPosition(0, hit.point);
                       
                    }
                    break;

                }
            }
        }
    }

    /// <summary>
    /// Combine Colors 
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    public static Color CombineColors(Color color1, Color color2)
    {

        Color result = new Color(0, 0, 0, 0);

        result = new Color((color1.r + color2.r) / 2f, (color1.g + color2.g) / 2f, (color1.b + color2.b) / 2f);
       
        return result;

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

                _startColor = CombineColors(_startColor, hitColor);
                for (int x = index; x <= _maxBounces; x++)
                {

                    _lineRenderers[index + 1].startColor = _startColor;
                    _lineRenderers[index + 1].endColor = _startColor;

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

        if(winColor == lineColor)
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

}
