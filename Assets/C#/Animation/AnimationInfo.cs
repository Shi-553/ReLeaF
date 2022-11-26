using Animancer.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility{
    public class AnimationInfoBase<T> : ScriptableObject where T:Enum
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

        [SerializeField]
        AnimationPair[] clips;

        Dictionary<T, AnimationPair> map=null;
        Dictionary<T, AnimationPair> Map => map ??= clips.ToDictionary(c => c.type, c => c);

        public AnimationPair GetPair(T type) => Map[type];
    }

}
