using UnityEngine;
using System.Collections;

public class LaunchAppButton : MonoBehaviour {

    void OnButtonPressed()
    {
        transform.root.Find("Configuration").GetComponent<Configuration>().LaunchApp();
    }

}
