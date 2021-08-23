using System;
using UnityEngine;

namespace Plugins.TransformInspector
{
    [Serializable]
    public class TransformSerialized
    {
        [SerializeField] internal bool local;
        [SerializeField] internal Vector3 Pos;
        [SerializeField] internal Quaternion Rot;
        [SerializeField] internal Vector3 Scale;
    
        public TransformSerialized(Transform tr,bool local = true)
        {
            if (!tr) return;
            this.local = local;
        
            if (local)
            {
                Pos = tr.localPosition;
                Rot = tr.localRotation;
            }
            else
            {
                Pos = tr.position;
                Rot = tr.rotation;
            }
            Scale = tr.localScale;    
        }

        public void Apply(Transform tr)
        {
            if (local)
            {
                tr.localPosition = Pos;
                tr.localRotation = Rot;
            }
            else
            {
                tr.position = Pos;
                tr.rotation = Rot;
            }
            tr.localScale = Scale;
        }
    }
}
