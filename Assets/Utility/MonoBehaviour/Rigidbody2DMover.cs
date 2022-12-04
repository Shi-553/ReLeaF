using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rigidbody2DMover : MonoBehaviour
    {
        Rigidbody2D rigid;
        Vector2 move;

        public bool UseUnScaledTime { get; set; } = false;
        float DeltaTime => UseUnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        public Vector2 Position { get => rigid.position; set => rigid.position = value; }

        public bool IsMove { get; private set; }

        private void Awake()
        {
            TryGetComponent(out rigid);
        }

        public void MoveDelta(Vector2 m)
        {
            move += DeltaTime * m;
        }
        private void FixedUpdate()
        {
            IsMove = move != Vector2.zero;

            rigid.MovePosition(Position + move);
            move = Vector2.zero;
        }

    }
}