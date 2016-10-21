using UnityEngine;

namespace Framework.Animation
{
    public abstract class AnimationTargeted : AnimationBase
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;

        public GameObject Target
        {
            get { return _target; }
            set
            {
                _target = value;
                if (_target != null)
                    OnSetTarget(_target);
            }
        }
        protected float Duration { get { return _duration; } }
        protected AnimationCurve Curve { get { return _curve; } }

        protected virtual void Awake()
        {
            Target = _target;
        }

        protected virtual void OnSetTarget(GameObject target) { }
    }
}
