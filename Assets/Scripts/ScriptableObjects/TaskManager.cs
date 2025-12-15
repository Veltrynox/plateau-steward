using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "PS/Task")]
public class GameTask : ScriptableObject
{
    public string ID;
    public string title;
    [TextArea] public string description;
    public bool isCompleted;
}