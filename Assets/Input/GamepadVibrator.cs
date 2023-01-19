using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utility;

namespace ReLeaf
{
    public class GamepadVibrator : SingletonBase<GamepadVibrator>
    {

        public enum VibrationStrength
        {
            Weak,
            Normal,
            Strong
        }
        [Serializable]
        struct VibrationStrengthSerialize
        {
            public VibrationStrength strength;
            public float power;
        }

        [SerializeField]
        VibrationStrengthSerialize[] vibrationStrengths;
        Dictionary<VibrationStrength, VibrationStrengthSerialize> vibrationStrengthDic;



        public override bool DontDestroyOnLoad => true;
        protected override void Init(bool isFirstInit, bool callByAwake)
        {
            if (isFirstInit)
            {
                vibrationStrengthDic = vibrationStrengths.ToDictionary(v => v.strength, v => v);

            }
        }
        protected override void UninitBeforeSceneUnload(bool isDestroy)
        {
            AllStop();
        }
        protected override void UninitAfterSceneUnload(bool isDestroy)
        {
            AllStop();
        }
        void AllStop()
        {
            waits.Clear();
            Gamepad.current.SetMotorSpeeds(0, 0);
            isCurrentVibration = false;
        }

        private void Start()
        {
            var playerInput = EventSystem.current.GetComponent<PlayerInput>();
            UpdateIsGamepad(playerInput);
            playerInput.onControlsChanged += UpdateIsGamepad;
        }

        bool isGamePad = false;
        private void UpdateIsGamepad(PlayerInput playerInput)
        {
            isGamePad = playerInput.currentControlScheme == "Gamepad";
        }

        HashSet<VibrationState> waits = new();
        VibrationState current;
        bool isCurrentVibration = false;

        bool needUpdate = false;

        public bool Vibrate(VibrationStrength strength, float duration)
        {
            if (!isGamePad)
            {
                return false;
            }

            var state = new VibrationState(strength, duration);
            if (!waits.Add(state))
                return false;

            if (!isCurrentVibration || current < state)
                needUpdate = true;

            return true;
        }
        public bool Vibrate(VibrationStrength strength, float duration, out VibrationState state)
        {
            if (!isGamePad)
            {
                state = default;
                return false;
            }

            state = new VibrationState(strength, duration);
            if (!waits.Add(state))
                return false;

            if (!isCurrentVibration || current < state)
                needUpdate = true;

            return true;
        }
        public bool FinishVibrate(VibrationState state)
        {
            if (!isGamePad)
            {
                return false;
            }

            if (current == state)
            {
                needUpdate = true;
                isCurrentVibration = false;
                return true;
            }

            return waits.Remove(state);
        }

        private void Update()
        {
            if (!isGamePad)
            {
                if (waits.Count != 0)
                    waits.Clear();
                return;
            }

            if (isCurrentVibration && current.finishTime < Time.time)
                needUpdate = true;

            if (!needUpdate)
                return;

            needUpdate = false;

            if (waits.Count == 0)
            {
                isCurrentVibration = false;
                Gamepad.current.SetMotorSpeeds(0, 0);
                return;
            }

            isCurrentVibration = true;

            current = waits.Max();

            waits.RemoveWhere(v => v.finishTime <= current.finishTime);

            var power = vibrationStrengthDic[current.strength].power;
            Gamepad.current.SetMotorSpeeds(power, power);
        }


        public readonly struct VibrationState : IEquatable<VibrationState>, IComparable<VibrationState>
        {
            public readonly VibrationStrength strength;
            public readonly float finishTime;

            public VibrationState(VibrationStrength strength, float duration)
            {
                this.strength = strength;
                finishTime = Time.time + duration;
            }

            public int CompareTo(VibrationState other)
            {
                var s = (int)strength - (int)other.strength;
                if (s != 0)
                    return s;

                return (int)(finishTime - other.finishTime);
            }

            public override bool Equals(object obj)
            {
                return obj is VibrationState state && Equals(state);
            }

            public bool Equals(VibrationState other)
            {
                return strength == other.strength &&
                       finishTime == other.finishTime;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(strength, finishTime);
            }

            public static bool operator ==(VibrationState left, VibrationState right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(VibrationState left, VibrationState right)
            {
                return !(left == right);
            }

            public static bool operator <(VibrationState left, VibrationState right)
            {
                return left.CompareTo(right) < 0;
            }

            public static bool operator >(VibrationState left, VibrationState right)
            {
                return left.CompareTo(right) > 0;
            }

            public static bool operator <=(VibrationState left, VibrationState right)
            {
                return left.CompareTo(right) <= 0;
            }

            public static bool operator >=(VibrationState left, VibrationState right)
            {
                return left.CompareTo(right) >= 0;
            }
        }
    }
}
