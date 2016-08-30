using UnityEngine;
using System.Collections;

public class LaunchButton : MonoBehaviour {

    void OnButtonPressed()
    {
        transform.root.Find("Configuration").GetComponent<Configuration>().BuyDevice();
    }

}
