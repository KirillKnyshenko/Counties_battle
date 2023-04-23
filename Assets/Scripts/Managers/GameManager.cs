using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    private enum State {
        TapToStart,
        TutorialToStart,
        GamePlaying,
        GameOver,
    }

    private State _state;
    private int _level;
    public int level => _level;

    [SerializeField] private LevelManager _levelManager;
    public LevelManager levelManager => _levelManager;

    [SerializeField] private UIManager _UIManager;
    public UIManager UIManager => _UIManager;

    [SerializeField] private SaveDataSO _saveData;
    [SerializeField] private LevelListSO _levelListSO;
    
    public UnityEvent OnStartLevel;
    public UnityEvent OnTapToStart;
    public UnityEvent OnTutorialToStart;
    public UnityEvent OnGameStarted;
    public UnityEvent OnGameOver;
    public UnityEvent OnWin;
    public UnityEvent OnLose;

    private void Start()
    {
        _state = State.TapToStart;
        _levelManager.Init(this);

        int currentLevel = _saveData.level + (_levelListSO.levels.Count * (_saveData.loop - 1));

        _level = currentLevel;

        _UIManager.Init(this, _level);
        StartCoroutine(GameManagerUpdate());

        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
    }

    private IEnumerator GameManagerUpdate() {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Lose();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Win();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _state = State.GameOver;
            }

            switch (_state)
            {
                case State.TapToStart:

                break;
                case State.TutorialToStart:
                    
                break;
                case State.GamePlaying:        
                    if (levelManager.players[0].playerMass <= 0f)
                        Lose();

                    float enemyMass = 0f;
                    for (int i = 1; i < levelManager.players.Count; i++)
                    {
                        enemyMass += levelManager.players[i].playerMass;
                    }
                    if (enemyMass == 0f)
                        Win();
                break;
                case State.GameOver:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        NextLevel();
                    }
                break;
            }
            yield return null;
        }
    }

    public void Win() {
        OnWin?.Invoke();
        _state = State.GameOver;
    }

    public void Lose() {  
        OnLose?.Invoke();
        _state = State.GameOver;
    }

    public void NextLevel() {
        OnStartLevel?.Invoke();
        _levelListSO.GetNextLevel();
        _saveData.Save();
        LoadCurrentLevel();
    }

    public void RestartLevel() {
        OnStartLevel?.Invoke();
        LoadCurrentLevel();
    }

    public void StartTutorialToStart() {
        _state = State.TutorialToStart;
        OnTutorialToStart?.Invoke();
    }

    public void StartGamePlaying() {
        _state = State.GamePlaying;
        OnGameStarted?.Invoke();
    }

    public void LoadCurrentLevel() {
        SceneManager.LoadScene(_levelListSO.GetCurrentLevelName());
    }

    public bool IsTutorialToStart() {
        return _state == State.TutorialToStart;
    }

    public bool IsGamePlaying() {
        return _state == State.GamePlaying;
    }

    public bool IsGameOver() {
        return _state == State.GameOver;
    }
}
