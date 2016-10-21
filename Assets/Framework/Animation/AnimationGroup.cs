using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Framework.Animation
{
    public class AnimationGroup : AnimationBase
    {
        [SerializeField]
        private bool _enableLogging;

        public event Action<AnimationGroup> Completed;

        private List<AnimationBase> _animations;
        private AnimationQueue _queue;

        void Awake()
        {
            _animations = GetComponents<AnimationBase>().Where(x => x != this).ToList();

            _queue = new AnimationQueue();
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

        protected override void OnPlay()
        {
            base.OnPlay();
            _queue.UpdateQueue();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _animations.ForEach(x => x.Stop());
        }

        void OnValidate()
        {
            if (_queue != null)
                _queue.EnableLogging = _enableLogging;
        }

        void OnDestroy()
        {
            _queue.Completed -= OnQueueComplete;
            _queue.Clear();
        }
    }
}
