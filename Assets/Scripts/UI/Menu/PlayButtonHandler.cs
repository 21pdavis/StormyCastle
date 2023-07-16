using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonHandler : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        // unsure of exact reasons, but LoadSceneAsync seems to not support the await syntax, so we have to use the callback (like .then() in JS)
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Game");

        // await the load operation's completion, only switch after it is done
        loadOperation.completed += (AsyncOperation operation) =>
        {
            Debug.Log("Finished Loading Game Scene");
            StateManager.Instance.SetState(StateManager.GameState.IntroCutscene);
        };
    }
}
