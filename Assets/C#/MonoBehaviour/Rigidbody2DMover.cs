using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReLeaf
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rigidbody2DMover : MonoBehaviour
    {
        Rigidbody2D rigid;
        Vector2 move;

        public bool UseUnScaledTime { get; set; } = false;
        float DeltaTime => UseUnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        public Vector2 Position => rigid.position;

        private void Awake()
        {
            TryGetComponent(out rigid);
        }

        public void Move(Vector2 m)
        {
            move += DungeonManager.CELL_SIZE * DeltaTime * m;
        }
        public void MoveTowards(Vector2 target, float speed)
        {
            move = Vector2.MoveTowards(Position, target , DeltaTime * speed * DungeonManager.CELL_SIZE)- Position;
        }
        private void FixedUpdate()
        {
            rigid.MovePosition(Position + move);
            move = Vector2.zero;
        }
    }
}