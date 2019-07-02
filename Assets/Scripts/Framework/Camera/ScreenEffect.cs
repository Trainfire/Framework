using UnityEngine;
using System;

namespace Framework.Components
{
    public abstract class ScreenEffect : MonoBehaviour
    {
        public bool Finished { get; private set; }

        public abstract void ProcessEffect();
        public abstract void Activate();
        public abstract void Deactivate();

        protected void Finish()
        {
            Finished = true;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
    }
}
