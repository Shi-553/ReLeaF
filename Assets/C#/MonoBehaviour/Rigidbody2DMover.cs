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
        public Vector2 Position { get => rigid.position; set => rigid.position = value; }

        private void Awake()
        {
            TryGetComponent(out rigid);
        }

        public void Move(Vector2 m)
        {
            move += DungeonManager.CELL_SIZE * DeltaTime * m;
        }
        private void FixedUpdate()
        {
            rigid.MovePosition(Position + move);
            move = Vector2.zero;
        }

        public void MovePotision(Vector2 pos)
        {
            rigid.MovePosition(pos);
            move = Vector2.zero;
        }
    }
}