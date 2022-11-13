using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSystem.Util
{
    [Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] 
        private List<KeyPair<TKey,TValue>> rawData = new List<KeyPair<TKey, TValue>>();

        public void OnBeforeSerialize()
        {
            rawData = this.Select(kv => new KeyPair<TKey, TValue> {key = kv.Key, value = kv.Value}).ToList();
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            foreach (var keyPair in rawData.Distinct())
            {
                Add(keyPair.key, keyPair.value);
            }
        }
    }

    public static class SDictionaryExtensions
    {

        public static SDictionary<TKey, TValue> ToSDictionary<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc,
            Func<T, TValue> valueFunc)
        {
            var dict = new SDictionary<TKey, TValue>();
            foreach (var keyPair in enumerable.Distinct())
            {
                dict.Add(keyFunc(keyPair), valueFunc(keyPair));
            }

            return dict;
        }
        
        public static SDictionary<TKey, TValue> ToSDictionary<TKey, TValue>(this IDictionary<TKey, TValue> enumerable)
        {
            var dict = new SDictionary<TKey, TValue>();
            foreach (var keyPair in enumerable.Distinct())
            {
                dict.Add(keyPair.Key, keyPair.Value);
            }
            return dict;
        }
    }

    [Serializable]
    public class KeyPair<TKey, TValue>
    {
        [SerializeField]
        public TKey key;
        
        [SerializeField]
        public TValue value;

        protected bool Equals(KeyPair<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(key, other.key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((KeyPair<TKey, TValue>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(key);
        }

        public static bool operator ==(KeyPair<TKey, TValue> left, KeyPair<TKey, TValue> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(KeyPair<TKey, TValue> left, KeyPair<TKey, TValue> right)
        {
            return !Equals(left, right);
        }
    }
}