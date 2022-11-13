using UnityEngine;

namespace GameSystem.Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
      
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
                Instance = (T)this;
                OnAwake();
            }
        }

        protected abstract void OnAwake();
    }
}