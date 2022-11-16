using System;
using UnityEngine;

namespace GameSystem.SaveLoad.TypeHandling
{
    public abstract class TypeSaver<T> where T : Component
    {

        public Type Type => typeof(T);

        public abstract string Serialise(T data);

        public abstract void DeSerialise(string data, T target);
    }
}