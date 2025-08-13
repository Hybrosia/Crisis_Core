using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    public void ShowSettings()
    {
        SettingsController.ShowSettings();
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
