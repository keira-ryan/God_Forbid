using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    Start = 0,
    Pause = 2,
    GameOver = 3,
    Playing = 1, 
    Dialogue = 4
}

public class GameController : MonoBehaviour
{
    [SerializeField] private InputManagement.InputManager inputManager;
    
    public static GameController Instance;
    
    public GameState CurrentGameState;
    public static event Action<GameState> OnGameStateChange;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameState(GameState.Start);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameStateInt(int state)
    {
        SetGameState((GameState)state);
    }

    public void SetGameState(GameState newGameState)
    {
        if (CurrentGameState == newGameState) return;
        CurrentGameState = newGameState;
        OnGameStateChange?.Invoke(newGameState);

        switch (newGameState)
        {
            case GameState.Start:
                inputManager.SetEnabled(false);
                inputManager.SwitchInputMode(InputManagement.InputManager.InputMode.UI);
                Time.timeScale = 0;
                UIController.Instance.ShowStartScreen();
                break;
            case GameState.Pause:
                inputManager.SetEnabled(false);
                inputManager.SwitchInputMode(InputManagement.InputManager.InputMode.UI);
                Time.timeScale = 0;
                UIController.Instance.ShowPauseScreen();
                break;
            case GameState.GameOver:
                inputManager.SetEnabled(false);
                inputManager.SwitchInputMode(InputManagement.InputManager.InputMode.UI);
                Time.timeScale = 0;
                UIController.Instance.ShowEndScreen();
                break;
            case GameState.Playing:
                inputManager.SetEnabled(true);
                inputManager.SwitchInputMode(InputManagement.InputManager.InputMode.Player);
                Time.timeScale = 1;
                UIController.Instance.ShowPlayUI();
                break;
            case GameState.Dialogue:
                inputManager.SetEnabled(false);
                inputManager.SwitchInputMode(InputManagement.InputManager.InputMode.UI);
                Time.timeScale = 1;
                UIController.Instance.ShowDialogue();
                break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SetGameState(GameState.Pause);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetGameState(GameState.GameOver);
                }
                break;
            case GameState.Pause:
                if (Input.GetKeyDown(KeyCode.P))
                {
                    SetGameState(GameState.Playing);
                }
                break;
        }
    }

    public void OnQuit()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
