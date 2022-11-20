using System;
using UnityEngine;

namespace Simpson.Character
{
    [CreateAssetMenu(fileName = "Move Config", menuName = "character/Move Config", order = 0)]
    public class MovementConfig : ScriptableObject, IComparable<MovementConfig>
    {
        [field:SerializeField]
        public int Priority { get; private set; }
        
        
        [field:SerializeField]
        public float Speed { get; private set; }
        [field:SerializeField]
        public float Acceleration { get; private set; }
        [field:SerializeField]
        public float TurnAcceleration { get; private set; }
        [field:SerializeField]
        public bool UseRootMotion { get; private set; }
        [field:SerializeField]
        public bool UseRootRotation { get; private set; }
        [field:SerializeField]
        public bool CanFall { get; private set; }

        public int CompareTo(MovementConfig other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return Priority.CompareTo(other.Priority);
        }
    }
}