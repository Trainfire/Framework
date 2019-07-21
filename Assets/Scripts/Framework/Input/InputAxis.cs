using System;
using UnityEngine;

namespace Framework
{
    public interface IInputAxis<TDelta>
    {
        string Name { get; }
        TDelta Delta { get; }
    }

    public class InputSingleAxis : IInputAxis<float>
    {
        public string Name { get; private set; }
        public string UnityAxis { get; private set; }

        public float Delta
        {
            get
            {
                var delta = Input.GetAxis(UnityAxis);

                if (Invert) delta = delta * -1;

                return delta;
            }
        }

        public bool Invert { get; set; }

        public InputSingleAxis(string name, string unityAxis)
        {
            Name = name;
            UnityAxis = unityAxis;
        }
    }

    /// <summary>
    /// Combines two axes (one horizontal and one vertical).
    /// </summary>
    public class InputTwinAxes : IInputAxis<Vector2>
    {
        public string Name { get; private set; }
        public string UnityXAxis { get; private set; }
        public string UnityYAxis { get; private set; }

        public Vector2 Delta
        {
            get
            {
                var x = Input.GetAxis(UnityXAxis);
                var y = Input.GetAxis(UnityYAxis);

                if (InvertX) x = x * -1;
                if (InvertY) y = y * -1;

                return new Vector2(x, y);
            }
        }

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public InputTwinAxes(string name, string unityXAxis, string unityYAxis)
        {
            Name = name;
            UnityXAxis = unityXAxis;
            UnityYAxis = unityYAxis;
        }
    }
}
