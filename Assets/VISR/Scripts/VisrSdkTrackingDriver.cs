using UnityEngine;
using System.Collections.Generic;
using System;

namespace VisrSdk
{
    public abstract class SdkTrackingDriver
    {

#if UNITY_WSA_8_1
        private static SdkTrackingDriver instance = new VisrSdkTrackingDriverUApp();
#else
        private static SdkTrackingDriver instance = new SdkTrackingDriverDefault();
#endif

        public static SdkTrackingDriver Instance
        {
            get { return instance; }
        }

        public abstract void Init();
        public abstract void UpdateTracking();
        public abstract string[] GetTrackingNodeNames();
        public abstract string GetDefaultCameraMountName();
        public abstract IEnumerable<TrackingNode> GetAllTrackingNodes();
        public abstract TrackingNode CreateTrackingNode(string name);
        public abstract TrackingNode GetTrackingNode(string name);
        public abstract TrackingNode GetTrackingNode(int id);

        private class SdkTrackingDriverDefault : SdkTrackingDriver
        {
            Dictionary<string, TrackingNode> trackingNodes = new Dictionary<string, TrackingNode>();
            
            //editor gyro emulation
            Vector3 lastMouse = Vector3.zero;

            void updateHeadTracking()
            {
                TrackingNode head = trackingNodes[TrackingNode.HEAD];

                if (Application.isEditor)
                {
                    if(Input.GetMouseButtonDown(1))
                    {
                        lastMouse = Input.mousePosition;
                    }

                    if (!Input.GetMouseButton(1))
                        return;

                    Vector3 deltaMouse = Input.mousePosition - lastMouse;
                    lastMouse = Input.mousePosition;
                    
                    head.Rotation = head.Rotation
                        * Quaternion.AngleAxis(-deltaMouse.y, Vector3.right)
                        * Quaternion.AngleAxis(deltaMouse.x, Vector3.up);

                    head.Rotation.eulerAngles = new Vector3(head.Rotation.eulerAngles.x, head.Rotation.eulerAngles.y, 0);
                    return;
                }

                Vector3 screenRotation = new Vector3(-90, 0, 90);
                Vector3 rotationRate = Vector3.zero;
                Vector3 accel = default(Vector3);

                rotationRate = Input.gyro.rotationRate * Time.deltaTime;
                head.Rotation = head.Rotation * Quaternion.Euler(-rotationRate.x, -rotationRate.y, rotationRate.z);

                if(accel == default(Vector3))
                    accel = Input.acceleration;

                //perform orientation transform for landscape left
                accel = new Vector3(-accel.x, accel.y, accel.z);

                Vector3 pitchVector = Vector3.ProjectOnPlane(accel, Vector3.right).normalized;
                Vector3 rollVector = Vector3.ProjectOnPlane(accel, Vector3.forward).normalized;

                float pitch = Mathf.Atan2(-pitchVector.y, pitchVector.z) * Mathf.Rad2Deg;
                float roll = Mathf.Atan2(rollVector.y, rollVector.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.Euler(new Vector3(pitch, head.Rotation.eulerAngles.y, roll) + screenRotation);

                //This represents a basic complimentary filter, it's not good long term we should use a Kalman filter here [ld]
                head.Rotation = Quaternion.Slerp(head.Rotation, rotation, 1.0F * Time.deltaTime);
            }

            public override void Init()
            {
                if (!SystemInfo.supportsGyroscope)
                    Debug.Log("No Gyro Detected, tracking may not work properly");

                Input.gyro.enabled = true;
                Input.gyro.updateInterval = 1.0f / 100.0f;

                trackingNodes[TrackingNode.HEAD] = new TrackingNode();
            }

            public override void UpdateTracking()
            {
                updateHeadTracking();
            }

            public override TrackingNode GetTrackingNode(string name)
            {
                TrackingNode node = null;

                if (trackingNodes.TryGetValue(name, out node))
                {
                    return node;
                }

                return null;
            }

            //Some Vicon Systems only have numeric tracking available
            public override TrackingNode GetTrackingNode(int id)
            {
                return GetTrackingNode(id.ToString());
            }

            public override TrackingNode CreateTrackingNode(string name)
            {
                TrackingNode node = new TrackingNode();
                trackingNodes[name] = node;
                return node;
            }

            public override string[] GetTrackingNodeNames()
            {
                string[] keys = new string[trackingNodes.Count];

                int i = 0;
                foreach(var key in trackingNodes.Keys)
                {
                    keys[i] = key;
                    ++i;
                }

                return keys;
            }

            public override IEnumerable<TrackingNode> GetAllTrackingNodes()
            {
                return trackingNodes.Values;
            }

            public override string GetDefaultCameraMountName()
            {
                return "Head";
            }
        }
    }

}