using UnityEngine;
using System.Collections.Generic;

namespace Framework.Animation
{
    public class AnimationRepeater : AnimationBase
    {
        [SerializeField] private float _delay;
        [SerializeField] private GameObject _prototype;
        [SerializeField] private List<GameObject> _targets;

        private AnimationQueue _queue;

        void Awake()
        {
            if (_targets == null)
                _targets = new List<GameObject>();

            var protoAnim = _prototype.GetComponent<AnimationTargeted>();
            if (protoAnim == null)
                Debug.LogError("The prototype object has no targeted animation attached.");

            _queue = new AnimationQueue();
            _queue.EnableLogging = true;
            _queue.Completed += OnQueueComplete;

            foreach (var target in _targets)
            {
                var instance = Instantiate(_prototype).GetComponent<AnimationTargeted>();
                instance.Target = target;

                _queue.Add(instance);

                // Add delay if a value is assigned.
                if (_delay != 0f)
                {
                    var delayComp = instance.gameObject.AddComponent<AnimationDelay>();
                    delayComp.Duration = _delay;
                    _queue.Add(delayComp);
                }
            }
        }

        void OnQueueComplete(AnimationQueue obj)
        {
            TriggerPlayComplete();
        }

        protected override void OnPlay()
        {
            base.OnPlay();
            _queue.UpdateQueue();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _queue.Cancel();
        }

        void OnDestroy()
        {
            _queue.Completed -= OnQueueComplete;
            _queue.Clear();

            _targets.Clear();
        }
    }
}
