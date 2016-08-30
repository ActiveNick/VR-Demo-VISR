using UnityEngine;
using System.Collections;
using System;

namespace VisrSdk
{    
    public class TrackingNode
    {
        public Quaternion Rotation = Quaternion.Euler(0,0,0);
        public Vector3 LocalPosition = Vector3.zero;
        public Matrix4x4 Transform;
        public object Tag;

        //predefined node names
        public const string HEAD = "head";
        public const string LHAND = "lhand";
        public const string RHAND = "rhand";
    }
}