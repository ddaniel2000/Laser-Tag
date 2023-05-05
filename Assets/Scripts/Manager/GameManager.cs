using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Laser _laser;
    [SerializeField]
    private GameObject _laserGameObject;
    [SerializeField]
    private GameObject[] _levels;
    [SerializeField]
    private Transform _levelsParent;
    [SerializeField]
    private Text _countdownText;
    [SerializeField]
    private GameObject _menuUi;
    [SerializeField]
    private AudioSource _audioSource;

    private int _countdown = 4;
    private int _levelIndex = -1;

    public static event Action onStartLevel;


    #region Subscribe/Unsubscribe Actions
    private void OnEnable()
    {

        Laser.onWinLevel += EndLevel;
        Laser.onWinLevel += StartTheMusic;
        
    }

    private void OnDisable()
    {

        Laser.onWinLevel -= EndLevel;
        Laser.onWinLevel -= StartTheMusic;

    }

    #endregion

    #region Lifecycle
    void Start()
    {

        StartCoroutine(OnLevelStart());
        
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            OnGamePaused();

        }
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
        _levelIndex++;
        PlayerPrefs.SetInt("Level",_levelIndex);
        PlayerPrefs.Save();

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

            if(_laser.enabled != true)
            {

                Instantiate(_levels[_levelIndex], _levelsParent);
                _laser.enabled = true;
                _laserGameObject.SetActive(true);
                onStartLevel?.Invoke();              
                _countdownText.enabled = false;

            }              
        }
    }

    /// <summary>
    ///  The process of ending the level
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnLevelEnd()
    {

        _laserGameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        _laser.enabled = false;
     

        if (_levelIndex == _levels.Length - 1)
        {

            _levelIndex = -1;

        }

        _countdown = 4;
        StartCoroutine(OnLevelStart());

    }

    /// <summary>
    /// Play the "Win" music
    /// </summary>
    private void StartTheMusic()
    {

        _audioSource.Play();

    }

    #endregion

    #region Public

    /// <summary>
    /// Pause Game
    /// </summary>
    public void OnGamePaused()
    {
        
        _menuUi.SetActive(!_menuUi.activeSelf);

    }

    /// <summary>
    /// Start a New Game with no data saved
    /// </summary>
    public void NewGame()
    {

        _levelIndex = -1;

    }

    /// <summary>
    /// Start game from the Last Saved level
    /// </summary>
    public void ContinueGame()
    {

        _levelIndex = PlayerPrefs.GetInt("Level") -1;

    }

    #endregion
}
