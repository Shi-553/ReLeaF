using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class CollectionWrapper<T>
    {
        [SerializeField]
        T value;
        public T Value => value;
        public CollectionWrapper(T value)
        {
            this.value = value;
        }
    }
    [Serializable]
    public class ArrayWrapper<T> : CollectionWrapper<T[]>
    {
        public ArrayWrapper(T[] value) : base(value)
        {
        }
    }
    [Serializable]
    public class ListWrapper<T> : CollectionWrapper<List<T>>
    {
        public ListWrapper(List<T> value) : base(value)
        {
        }
    }
}
