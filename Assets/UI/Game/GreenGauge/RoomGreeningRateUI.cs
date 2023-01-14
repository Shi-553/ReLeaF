using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ReLeaf
{
    public class RoomGreeningRateUI : SingletonBase<RoomGreeningRateUI>
    {
        [SerializeField]
        Slider slider;


        public override bool DontDestroyOnLoad => false;

        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (callByAwake)
            {
                gameObject.SetActive(false);
                PlayerMover.Singleton.OnChangeRoom += OnChangeRoom;
                return;
            }
        }

        private void OnChangeRoom(Room room)
        {
            if (room == null)
                Inactive();
            else
                Active();
        }

        public void Active()
        {
            gameObject.SetActive(true);
        }
        public void Inactive()
        {
            gameObject.SetActive(false);
        }
        public void SetValue(float value)
        {
            slider.value = value;
        }

    }
}
