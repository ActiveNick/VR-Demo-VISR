using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_WSA_8_1
using Windows.Devices.Sensors;
#endif

namespace VisrSdk
{
#if UNITY_WSA_8_1
    public class VisrSdkTrackingDriverUApp : SdkTrackingDriver
    {
        Dictionary<string, TrackingNode> trackingNodes = new Dictionary<string, TrackingNode>();

        OrientationSensor sensor;
        Gyrometer gyro;
        Accelerometer acro;

        void updateHeadTracking()
        {
            TrackingNode head = trackingNodes[TrackingNode.HEAD];

            Vector3 screenRotation = new Vector3(-90, 0, 90);
            Vector3 rotationRate = Vector3.zero;
            Vector3 accel = default(Vector3);

            var raw = gyro.GetCurrentReading();
            rotationRate = new Vector3((float)-raw.AngularVelocityX, (float)-raw.AngularVelocityY, (float)raw.AngularVelocityZ) * Time.deltaTime;
            head.Rotation = head.Rotation * Quaternion.Euler(-rotationRate.x, -rotationRate.y, rotationRate.z);

            if (accel == default(Vector3))
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

            sensor = OrientationSensor.GetDefault();
            gyro = Gyrometer.GetDefault();
            gyro.ReportInterval = gyro.MinimumReportInterval;
            acro = Accelerometer.GetDefault();

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
            foreach (var key in trackingNodes.Keys)
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
#endif
}
