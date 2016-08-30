using UnityEngine;
using UnityEditor;
using System.Collections;

using VisrSdk;

public class VRMenu : EditorWindow {

    const string TrackingSystemPath = "Assets/VISR/Prefabs/TrackingSystem.prefab";
    const string CameraPath = "Assets/VISR/Prefabs/VRCamera.prefab";

    [MenuItem("VR/Provision Scene %q")]
    static void ProvisionScene()
    {
        if(EditorUtility.DisplayDialog("Provision for VR", "This action will delete all current GameObject's in this scene\r\nare you sure?", "Yes", "No"))
        {
            foreach(GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
            {
                DestroyImmediate(gameObject);
            }

            LoadTrackingSystem();

            LoadCamera();

            GameObject light = new GameObject("Sunlight");
            light.transform.eulerAngles = new Vector3(45, 45, 0);
            light.AddComponent<Light>().type = LightType.Directional;
         }
    }

    [MenuItem("VR/Load Camera")]
    static void LoadCamera()
    {
        if (FindObjectsOfType<VisrSdk.VisrCamera>().Length > 0)
        {
            EditorUtility.DisplayDialog("Provision for VR", "A VRCamera already exists in this scene, skipping", "Ok");
            return;
        }

        GameObject cameraPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CameraPath);
        if (cameraPrefab == null)
        {
            Debug.LogError("Could not find camera prefab at " + CameraPath + " have you moved things around?");
            return;
        }

        GameObject camera = GameObject.Instantiate(cameraPrefab);
        camera.name = "VRCamera";
        camera.transform.position = Vector3.zero;
    }

    [MenuItem("VR/Load Tracking System")]
    static void LoadTrackingSystem()
    {
        if (FindObjectsOfType<TrackingSystem>().Length > 0)
        {
            EditorUtility.DisplayDialog("Provision for VR", "A TrackingSystem already exists in this scene, skipping", "Ok");
            return;
        }

        GameObject trackingSystemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrackingSystemPath);
        if (trackingSystemPrefab == null)
        {
            Debug.LogError("Could not find base TrackingSystem prefab at " + TrackingSystemPath + " have you moved things around?");
            return;
        }

        GameObject trackingSystem = GameObject.Instantiate(trackingSystemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        trackingSystem.name = "TrackingSystem";

        if (trackingSystem == null)
        {
            Debug.LogError("Error creating tracking system from " + TrackingSystemPath);
            return;
        }
    }

    void OnGUI()
    {
        
    }
}

[CustomEditor(typeof(TrackingSystem))]
public class TrackingSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Button("TEST");
        
    }
}
