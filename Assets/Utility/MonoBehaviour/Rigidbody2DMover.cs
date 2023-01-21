using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rigidbody2DMover : MonoBehaviour
    {
        Rigidbody2D rigid;

        public bool UseUnScaledTime { get; set; } = false;
        float DeltaTime => UseUnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        public Vector2 Position { get => rigid.position; set => rigid.position = value; }

        public bool IsMove { get; private set; }

        private void Awake()
        {
            TryGetComponent(out rigid);

        }

        Vector2 move;
        Vector2 beforeMove;
        public void MoveDelta(Vector2 m)
        {
            move += m;
        }
        private void LateUpdate()
        {
            beforeMove = move;
            move = Vector2.zero;
        }
        private void FixedUpdate()
        {
            IsMove = beforeMove != Vector2.zero;

            if (IsMove)
                rigid.MovePosition(Position + DeltaTime * beforeMove);
        }

    }
}