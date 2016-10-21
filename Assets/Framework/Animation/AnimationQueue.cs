using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Animation
{
    public class AnimationQueue : MonoBehaviour
    {
        public event Action<AnimationQueue> Completed;

        private List<AnimationBase> _animations;

        public bool EnableLogging { get; set; }

        void Awake()
        {
            _animations = new List<AnimationBase>();
        }

        public void Add(AnimationBase animation)
        {
            if (!_animations.Contains(animation))
            {
                animation.Triggered += OnAnimationEvent;
                _animations.Add(animation);
            }
        }

        public void Add(List<AnimationBase> animations)
        {
            animations.ForEach(x => Add(x));
        }

        public void Remove(AnimationBase animation)
        {
            if (_animations.Contains(animation))
                _animations.Remove(animation);
        }

        public void Clear()
        {
            foreach (var anim in _animations)
            {
                if (anim != null)
                    anim.Triggered -= OnAnimationEvent;
            }

            _animations.Clear();
        }

        public void UpdateQueue()
        {
            var playbackQueue = _animations.Where(x => x.State == AnimationPlaybackState.Stopped).ToList();

            foreach (var item in playbackQueue)
            {
                Log("Playing '" + item.GetType().Name + "'");
                item.Play();

                if (item.WaitForCompletion)
                {
                    Log("Waiting for '" + item.GetType().Name + "'");
                    break;
                }
            }
        }

        void OnAnimationEvent(AnimationEvent obj)
        {
            if (obj.PlaybackType == AnimationEventType.PlayComplete)
            {
                if (_animations.TrueForAll(x => x.State == AnimationPlaybackState.PlayComplete))
                {
                    Completed.InvokeSafe(this);
                }
                else
                {
                    UpdateQueue();
                }
            }
        }

        void Log(string message, params object[] args)
        {
            if (EnableLogging)
                Debug.LogFormat("{0}: {1}", "AnimationQueue", string.Format(message, args));
        }

        void OnDestroy()
        {
            Clear();
        }
    }
}
