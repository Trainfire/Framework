using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Converts a Unity Input Axis so it works like a button with down, hold, and up events.
    /// </summary>
    public class InputUnityAxisToButtonConverter
    {
        public string UnityAxis { get; private set; }
        public bool Inverted { get; private set; }
        public InputActionType LastAction { get; private set; }
        public float Delta { get; private set; }

        /// <summary>
        /// Returns true if the state changed during the last update.
        /// </summary>
        public bool DidUpdate { get; private set; }

        public InputUnityAxisToButtonConverter(string unityAxis, bool inverted = false)
        {
            UnityAxis = unityAxis;
            Inverted = inverted;
        }

        public void Update()
        {
            var thisFrameValue = Input.GetAxis(UnityAxis);
            var thisFrameAction = InputActionType.None;
            var isAboveThreshold = Inverted ? thisFrameValue < 0.0f : thisFrameValue > 0.0f;

            DidUpdate = false;

            if (isAboveThreshold)
            {
                if (LastAction == InputActionType.None)
                {
                    thisFrameAction = InputActionType.Down;
                }
                else
                {
                    thisFrameAction = InputActionType.Held;
                }
            }
            else if (LastAction == InputActionType.Held)
            {
                thisFrameAction = InputActionType.Up;
            }

            if (LastAction != thisFrameAction)
                DidUpdate = true;

            LastAction = thisFrameAction;
            Delta = thisFrameValue;
        }
    }
}
