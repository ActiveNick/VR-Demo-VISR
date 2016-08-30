using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ConfigurationEditorMonitor : MonoBehaviour {

#if UNITY_EDITOR
    void Start()
    {
        if(UnityEditor.EditorUtility.DisplayDialog("Information", "This is the device config scene, don't load this scene manually. It is automatically loaded when you execute your application on the device.\r\nIf you get scene not found errors when playing ensure this scene is added to the \"Scenes In Build\" list in position 0", "OK"))
        {
            UnityEditor.EditorUtility.DisplayDialog("Information", "When you go to deploy the app, set the Default Level Name field to your first scene", "OK");
            UnityEditor.Selection.activeGameObject = gameObject;
        }
    }
#endif
}
