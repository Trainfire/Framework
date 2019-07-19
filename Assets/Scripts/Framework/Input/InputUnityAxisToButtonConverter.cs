using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Maps an input axis to an action.
    /// </summary>
    public class InputUnityAxisToButtonConverter
    {
        public string UnityAxis { get; private set; }
        public bool Inverted { get; private set; }
        public InputActionType LastAction { get; private set; }
        public float Delta { get; private set; }

        public InputUnityAxisToButtonConverter(string unityAxis, bool inverted = false)
        {
            UnityAxis = unityAxis;
            Inverted = inverted;
        }

        /// <summary>
        /// Returns true for the frame this axis changed from 0.0f to > 0.0f
        /// </summary>
        /// <returns></returns>
        public void Update()
        {
            var thisFrameValue = Input.GetAxis(UnityAxis);
            var thisFrameAction = InputActionType.None;
            var isAboveThreshold = Inverted ? thisFrameValue < 0.0f : thisFrameValue > 0.0f;

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

            LastAction = thisFrameAction;
            Delta = thisFrameValue;
        }
    }
}
