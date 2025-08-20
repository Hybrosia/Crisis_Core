using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionSelect;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer audioMixer;
    private static Canvas _settingsCanvas;

    public static void ShowSettings()
    {
        _settingsCanvas.enabled = true;
    }
    
    private void Awake()
    {
        _settingsCanvas = GetComponent<Canvas>();
        fullscreenToggle.SetIsOnWithoutNotify(true);
    }
    
    public void ToggleFullscreen(bool setting)
    {
        Screen.fullScreenMode = setting ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetResolution(int index)
    {
        if (index == 0) return;
        
        var splitString = resolutionSelect.options[index].text.Split("x");
        
        Screen.SetResolution(int.Parse(splitString[0]), int.Parse(splitString[1]), Screen.fullScreenMode);
    }
    
    public void SetAAQuality(int index)
    {
        switch (index)
        {
            case 0:
                return;
            case 1:
                QualitySettings.antiAliasing = 0;
                break;
            case 2:
                QualitySettings.antiAliasing = 2;
                break;
            case 3:
                QualitySettings.antiAliasing = 4;
                break;
            case 4:
                QualitySettings.antiAliasing = 8;
                break;
        }
    }
    
    private void SetVolume(string parameter, float volume)
    {
        audioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
    
    public void SetMasterVolume(float volume)
    {
        SetVolume("Master", volume);
    }
    
    //Sets the sound effect volume.
    public void SetSfxVolume(float volume)
    {
        SetVolume("SFX", volume);
    }
    
    //Sets the music volume.
    public void SetMusicVolume(float volume)
    {
        SetVolume("Music", volume);
    }
    
    //Sets the ambience volume.
    public void SetAmbienceVolume(float volume)
    {
        SetVolume("Ambience", volume);
    }
    
    //Sets the ambience volume.
    public void SetVoiceVolume(float volume)
    {
        SetVolume("Voice", volume);
    }
}
