using UnityEngine;
using System.Collections;

namespace VisrSdk
{
    public class TrackingSystem : MonoBehaviour
    {
        static TrackingSystem instance = null;
        public static TrackingSystem Instance
        {
            get
            {
                if (Application.isEditor)
                {
                    if (instance == null)
                    {
                        instance = GameObject.Find("TrackingSystem").GetComponent<TrackingSystem>();
                    }
                }

                return instance;
            }
        }

        void Start()
        {
            if (instance != null)
            {
                Debug.LogError("Detected multiple TrackingSystem objects, deleting the old one.");
                Destroy(instance);
            }

            instance = this;
            SdkTrackingDriver.Instance.Init();
        }

        public void SyncToTrackingNode(string nodeName, Transform targetTransform)
        {
            if (!Application.isPlaying)
                return;

            Transform node = transform.FindChild(nodeName);
            if (node == null)
            {
                Debug.LogError("A tracking node with the name " + nodeName + " was not found");
                return;
            }

            targetTransform.position = node.position;
            targetTransform.rotation = node.rotation;
        }

        void LateUpdate()
        {
            SdkTrackingDriver.Instance.UpdateTracking();

            foreach (Transform child in transform)
            {
                TrackingNode node = SdkTrackingDriver.Instance.GetTrackingNode(child.name.ToLower());

                if (node != null)
                {
                    child.localPosition = node.LocalPosition;
                    child.rotation = node.Rotation;
                }
            }
        }
    }
}