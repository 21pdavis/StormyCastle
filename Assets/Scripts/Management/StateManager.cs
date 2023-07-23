using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static Helpers;

public class StateManager : MonoBehaviour
{
    // follow the singleton pattern for the manager, should access this using StateManager.Instance and never instantiate StateManager
    public static StateManager Instance { get; private set; }

    public enum GameState 
    {
        MainMenu,
        IntroCutscene,
        Playing,
        Paused,
    }

    public GameState CurrentState { get; private set; }

    private PlayerAwakening playerAwakening;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            // set the instance to this object if it is null
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetState(GameState.Playing);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"Changing state from {CurrentState} to {newState}");
        CurrentState = newState;
        switch (newState)
        {
            case GameState.MainMenu:
                // Load the main menu
                if (SceneManager.GetActiveScene().name != "MainMenu")
                {
                    SceneManager.LoadScene("MainMenu");
                }
                break;
            case GameState.IntroCutscene:
                playerAwakening = new();
                Time.timeScale = 0f;
                StartCoroutine(playerAwakening.AwakeningStart());
                StartCoroutine(FadeInSound(playerAwakening.rainSound));
                break;
            case GameState.Paused:
                // Pause the game
                // TODO add pause menu and fade in background
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                // Resume gameplay
                Time.timeScale = 1f;
                break;
        }
    }
}
