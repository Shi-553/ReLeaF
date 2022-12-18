using System.Linq;
using UnityEngine;
using Utility;

namespace ReLeaf
{
    public interface IRotateable
    {
        void Rotate(Quaternion rotation, Vector3 pos);
    }
    public class StageObject : TileObject, IMultipleVisual, IRotateable
    {
        enum StageObjectType
        {
            Bone,
            Fishbone,
            House1,
            House2,
            Ship1,
            Ship2,
            Solt1_1,
            Solt1_2,
            Solt2_1,
            Solt2_2,
            Max,
        }

        public override TileObject InstancedTarget => transform.GetComponentsInChildren<TileObject>().Last();

        [SerializeField]
        StageObjectType type;

        public int VisualType => type.ToInt32();
        public int VisualMax => StageObjectType.Max.ToInt32();

        public void Rotate(Quaternion rotation, Vector3 pos)
        {
            var model = transform.Find("model");
            model.localRotation = rotation;
            model.localPosition = pos;
        }
    }
}
