using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Laser _laser;
    [SerializeField]
    private GameObject _laserObject;
    [SerializeField]
    private GameObject[] _levels;
    [SerializeField]
    private Transform _levelsParent;
    [SerializeField]
    private Text _countdownText;

    private int _countdown = 4;
    private int _count = -1;

    public delegate void OnStartLevel();
    public static OnStartLevel onStartLevel;

    #region Lifecycle

    private void OnEnable()
    {
        Laser.onWinLevel += EndLevel;
    }

    private void OnDisable()
    {
        Laser.onWinLevel -= EndLevel;

    }

    void Start()
    {
        StartCoroutine(OnLevelStart());
    }

    #endregion

    #region Private

    /// <summary>
    /// End Level means to repeat the StartCoroutine because the game has no ending
    /// you just loop the levels
    /// </summary>
    private void EndLevel()
    {

        StartCoroutine(OnLevelEnd());

    }

    /// <summary>
    /// The process of starting the level
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnLevelStart()
    {

        _countdownText.enabled = true;
        _count++;
        
        // Start the Countdown
        while (_countdown > 1)
        {
            _countdown--;
            _countdownText.text = _countdown.ToString();
            yield return new WaitForSeconds(1);
        }

        // If the countdown is finished, activate the level
        if (_countdown == 1)
        {
            _countdownText.enabled = false;
            Instantiate(_levels[_count], _levelsParent);           
            _laser.enabled = true;
            onStartLevel?.Invoke();
            _laserObject.SetActive(true);
            
        }
    }

    /// <summary>
    ///  The process of ending the level
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnLevelEnd()
    {

        _laserObject.SetActive(false);       
        yield return new WaitForSeconds(2f);
        _laser.enabled = false;

        if (_count == _levels.Length - 1)
        {

            _count = -1;

        }

        _countdown = 4;
        StartCoroutine(OnLevelStart());

    }

    #endregion
}
