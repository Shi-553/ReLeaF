using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public class AnimationInfoBase<T> : ScriptableObject, ISerializationCallbackReceiver where T : Enum
    {
        [Serializable]
        public class AnimationPair
        {
            [SerializeField]
            public T type;
            [SerializeField]
            public AnimationClip left;
            [SerializeField]
            public AnimationClip right;

            public AnimationClip GetClip(bool isLeft) => isLeft ? left : right;
        }
        [Serializable]
        public class AnimationSingle
        {
            [SerializeField]
            public T type;
            [SerializeField]
            public AnimationClip clip;

            public AnimationClip GetClip() => clip;
        }

        [SerializeField]
        AnimationPair[] pairClips;
        [SerializeField]
        AnimationSingle[] singleClips;

        Dictionary<T, AnimationPair> pairClipMap = null;
        Dictionary<T, AnimationSingle> singleClipMap = null;


        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (pairClips != null)
                pairClipMap = pairClips.ToDictionary(c => c.type, c => c);
            if (singleClips != null)
                singleClipMap = singleClips.ToDictionary(c => c.type, c => c);
        }


        public AnimationClip GetClip(T type, bool isLeft) => pairClipMap[type].GetClip(isLeft);
        public AnimationClip GetClip(T type) => singleClipMap[type].GetClip();

    }

}
