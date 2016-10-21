using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Framework.Animation
{
    public class AnimationGroup : MonoBehaviour
    {
        [SerializeField]
        private bool _enableLogging;

        public event Action<AnimationGroup> Completed;

        private List<AnimationBase> _animations;
        private AnimationQueue _queue;

        void Awake()
        {
            _animations = GetComponents<AnimationBase>().ToList();

            _queue = gameObject.AddComponent<AnimationQueue>();
            _queue.Add(_animations);
            _queue.Completed += OnQueueComplete;
            _queue.EnableLogging = _enableLogging;
        }

        void OnQueueComplete(AnimationQueue obj)
        {
            if (_enableLogging)
                Debug.Log("Completed");

            Completed.InvokeSafe(this);    
        }

        public void Play()
        {
            _queue.UpdateQueue();
        }

        public void Stop()
        {
            _animations.ForEach(x => x.Stop());
        }

        void OnDestroy()
        {
            _queue.Completed -= OnQueueComplete;
            _queue.Clear();
        }
    }
}
