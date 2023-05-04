using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    private float _sensitivity = 330f;

    private void OnMouseDrag()
    {
        

        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime, Space.Self);


    }
}
