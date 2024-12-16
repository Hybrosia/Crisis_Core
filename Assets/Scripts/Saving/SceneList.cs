using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneList", menuName = "Scriptable Objects/SceneList")]
public class SceneList : ScriptableObject
{
    public List<string> Scenes;
}
