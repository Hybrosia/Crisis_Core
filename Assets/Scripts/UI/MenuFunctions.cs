using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public void ShowSettings()
    {
        SettingsController.ShowSettings();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
