using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueResponseEvents))]

public class DialogueResponseEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DialogueResponseEvents resposneEvents = (DialogueResponseEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            resposneEvents.OnValidate();
        }
    }
}
