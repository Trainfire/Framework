using UnityEngine;
using System;

namespace Framework
{
    public class Timer : MonoBehaviour
    {
        private Action<Timer> _callback;
        private float _duration;
        private float _timestamp;

        void Awake()
        {
            enabled = false;
        }

        public void Setup(float duration, Action<Timer> callback)
        {
            _duration = duration;
            _callback = callback;
            _timestamp = Time.time;

            enabled = true;
        }

        void Update()
        {
            if (Time.time > _timestamp + _duration)
            {
                _callback.Invoke(this);
                Destroy(gameObject);
            }
        }

        public static void Create(float duration, Action<Timer> callback)
        {
            var go = new GameObject();
            var timer = go.AddComponent<Timer>();
            timer.Setup(duration, callback);
        }
    }
}
