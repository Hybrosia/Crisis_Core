using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScript : MonoBehaviour
{
    public static SaveData CurrentSave;
    public static SceneList SceneList;

    //Makes the game object this is on not be destroyed when a new scene loads.
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneList = FindFirstObjectByType<SceneList>();
        
        LoadFromDisk();
    }
    
    //If a save exists, loads it. If not, uses a blank save.
    public static void LoadFromDisk()
    {
        var path = Application.persistentDataPath + Path.DirectorySeparatorChar + "save0.json";
        if (File.Exists(path))
        {
            CurrentSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }
        else
        {
            CurrentSave = new SaveData();
        }
        
        SceneManager.LoadScene(SceneList.Scenes[CurrentSave.LevelIndex]);
        
        //TODO: Set the player's spirit to the saved value.
    }

    //Writes the CurrentSave-object to a JSON-file on disk. Does NOT change any values, so any changes must be made before calling this.
    public static void WriteSaveToDisk()
    {
        var path = Application.persistentDataPath + Path.DirectorySeparatorChar + "save0.json";
        var json = JsonUtility.ToJson(CurrentSave);

        File.WriteAllText(path, json);
    }
}

//The object that represents a player's saved data.
public class SaveData
{
    public int LevelIndex;
    public int CheckpointIndex;
    public float Spirit;
}