using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDestroy : MonoBehaviour
{

    private void OnEnable()
    {
        Laser.onWinLevel += DestroyLevel;
    }

    private void OnDisable()
    {
        Laser.onWinLevel -= DestroyLevel;
    }

    private void DestroyLevel()
    {
        Destroy(gameObject, 0);
    }
}
