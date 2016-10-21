using UnityEngine;
using System;

namespace Framework.Animation
{
    public enum AnimationPlaybackState
    {
        Stopped,
        Playing,
        PlayComplete,
    }

    public enum AnimationEventType
    {
        None,
        Playing,
        Stopping,
        PlayComplete,
    }

    public class AnimationEvent : EventBase<AnimationBase>
    {
        public AnimationEventType PlaybackType { get; private set; }

        public AnimationEvent(AnimationBase sender, AnimationEventType playbackType) : base(sender)
        {
            PlaybackType = playbackType;
        }
    }

    public abstract class AnimationBase : MonoBehaviour
    {
        public event Action<AnimationEvent> Triggered;

        [SerializeField] private bool _awaitable;
        public bool Awaitable
        {
            get { return _awaitable; }
            protected set { _awaitable = value; }
        }

        public AnimationPlaybackState State { get; private set; }

        public void Play()
        {
            State = AnimationPlaybackState.Playing;
            Triggered.InvokeSafe(new AnimationEvent(this, AnimationEventType.Playing));
            OnPlay();
        }

        public void Stop()
        {
            State = AnimationPlaybackState.Stopped;
            Triggered.InvokeSafe(new AnimationEvent(this, AnimationEventType.Stopping));
            OnStop();
        }

        protected virtual void OnPlay() { }
        protected virtual void OnStop() { }

        protected void TriggerPlayComplete()
        {
            State = AnimationPlaybackState.PlayComplete;
            Triggered.InvokeSafe(new AnimationEvent(this, AnimationEventType.PlayComplete));
        }
    }
}
