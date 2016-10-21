using UnityEngine;

namespace Framework.Animation
{
    public abstract class AnimationTargeted : AnimationBase
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;

        protected GameObject Target { get { return _target; } }
        protected float Duration { get { return _duration; } }
        protected AnimationCurve Curve { get { return _curve; } }
    }
}
