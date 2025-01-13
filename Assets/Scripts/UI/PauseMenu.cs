using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += DisablePauseMenuOnSceneLoad;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DisablePauseMenuOnSceneLoad;
    }

    private void DisablePauseMenuOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        gameObject.SetActive(false);
    }
}
