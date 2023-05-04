using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LaserManager : MonoBehaviour
{
    static public LaserManager instance;
  
    public GameObject _linePrefab;
    private List<LaserGun> _lasers = new List<LaserGun>();
    private List<GameObject> _lines = new List<GameObject>();
    private float _maxStepDistance = 100;
    private Color _lineColor = new Color(1, 1, 1, 1);
    [SerializeField]
    private LaserGun laser;
    public static event Action onWinLevel;
    public void AddLaser(LaserGun laser) { _lasers.Add(laser); }

    public void RemoveLaser(LaserGun laser) { _lasers.Remove(laser); }

    private void RemoveOldLines(int linesCount)
    {
        if(linesCount < _lines.Count)
        {
            Destroy(_lines[_lines.Count - 1]);
            _lines.RemoveAt(_lines.Count - 1);
            RemoveOldLines(linesCount);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
      
    }

    // Update is called once per frame
    void Update()
    {
       
        int linesCount = 0;
        linesCount += CalculateLine(laser.transform.position + laser.transform.forward * 2f, laser.transform.forward, linesCount);

        RemoveOldLines(linesCount);

    }

    int CalculateLine(Vector3 startPosition, Vector3 direction,int index)
    {

        int result = 1;
        RaycastHit hit;
        Ray ray = new Ray(startPosition, direction);      
        bool intersect = Physics.Raycast(ray, out hit, _maxStepDistance);

        Vector3 hitPosition = hit.point;
        if(intersect)
        {
            if (hit.collider.CompareTag("Mirror"))
            {

                result += CalculateLine(hitPosition, Vector3.Reflect(direction, hit.normal), index + result);
            }
            else
            if (hit.collider.CompareTag("Enviroment"))
            {

            }
            else
            if (hit.collider.CompareTag("Glass"))
            {
                result += CalculateLine(hitPosition, hit.collider.transform.forward, index + result);
                //GetColor(hit, result);
            }
     
            
        }
        else
        {
            hitPosition = startPosition + direction * _maxStepDistance;
        }

       
       
        //if (intersect && hit.collider.CompareTag("Win"))
        //{
        //    result += CalculateLine(hitPosition, Vector3.zero, index + result);
        //    GetColor(hit, index);
        //}
        Debug.Log(result);
        DrawLine(startPosition, hitPosition,_lineColor, index);
        return result;
    }

    private void DrawLine(Vector3 startPosition, Vector3 finishPosition,Color color, int index)
    {
      
        LineRenderer line = null;
        if(index <_lines.Count)
        {
            line = _lines[index].GetComponent<LineRenderer>();
        }
        else
        {
            GameObject go = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
            line = go.GetComponent<LineRenderer>();
            
            _lines.Add(go);
            Debug.Log(index);
        }
       
        line.SetPosition(0, startPosition);
        line.SetPosition(1, finishPosition);
        line.startColor = color;
        line.endColor = color;
    }

    public static Color CombineColors(Color color1, Color color2)
    {

        Color result = new Color(0, 0, 0, 0);

        result = Color.Lerp(color1, color2, 0.5f);

        return result;

    }

    private void GetColor(RaycastHit hit, int index)
    {

        Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
        LineRenderer line = null;
        if (renderer != null)
        {
            Color hitColor = renderer.material.color;
            Debug.Log(index);
            line = _lines[index].GetComponent<LineRenderer>();
            if (hit.transform.tag != "Win")
            {
                _lineColor = CombineColors(_lineColor, hitColor);

                line.startColor = _lineColor;
                line.endColor = _lineColor;

            }
            else
            {
                CompareColorsToWin(hitColor, _lineColor);
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

        if (winColor == lineColor)
        {
            // Start WinLevel process
            onWinLevel?.Invoke();

        }

    }

}
