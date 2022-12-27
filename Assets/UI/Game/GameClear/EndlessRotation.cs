using UnityEngine;

namespace ReLeaf
{
    public class EndlessRotation : MonoBehaviour
    {
        [SerializeField]
        float rotateSpeed = 1;
        void Update()
        {
            transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        }
    }
}
