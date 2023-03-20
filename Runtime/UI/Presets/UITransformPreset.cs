using System;
using UnityEngine;

namespace Broccollie.UI
{
    [Serializable]
    public class UITransformPreset : UIBasePreset
    {
        public TransformSetting[] Settings = null;

        [Serializable]
        public class TransformSetting : Setting
        {
            [Header("Scale")]
            public bool IsScaleEnabled;
            public float TargetScale;
            public float ScaleDuration;
            public AnimationCurve ScaleCurve;
        }
    }
}