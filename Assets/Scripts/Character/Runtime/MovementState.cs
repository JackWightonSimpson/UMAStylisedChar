using System;

namespace Simpson.Character
{
    [Serializable]
    public class MovementState
    {
        public float speed;
        public float acceleration;
        public float turnAcceleration;

        public bool useRootMotion;
        public bool useRootRotation;
        
        public bool canFall = true;
    }
}