using System;
using UnityEngine;

namespace ReLeaf
{
    public class OnEnterChecker : MonoBehaviour
    {
        public event Action<Collider2D> OnEnter;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnEnter?.Invoke(collision);
        }
    }
}
