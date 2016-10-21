using UnityEngine;

namespace Framework.Animation
{
    class AnimationDelay : AnimationBase
    {
        [SerializeField] protected float _duration;
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private TweenFloat _tweener;

        void Awake()
        {
            Awaitable = true;
        }

        protected override void OnPlay()
        {
            base.OnPlay();

            if (_tweener != null)
                Destroy(_tweener);

            _tweener = gameObject.AddComponent<TweenFloat>();
            _tweener.From = 0f;
            _tweener.To = 1f;
            _tweener.OnDone += OnTweenDone;
            _tweener.Duration = _duration;
            _tweener.Play();
        }

        void OnTweenDone(Tween<float> obj)
        {
            obj.OnDone -= OnTweenDone;
            TriggerPlayComplete();
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (_tweener != null)
            {
                _tweener.Stop();
                _tweener.OnDone -= OnTweenDone;
            }
        }
    }
}
