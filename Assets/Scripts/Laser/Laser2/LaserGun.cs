using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LaserManager.instance.AddLaser(this);
    }

}
