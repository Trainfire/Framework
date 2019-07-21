using System;
using UnityEngine;

namespace Framework
{
    public interface IInputAxis
    {
        string Name { get; }
        void Update();
    }

    public class InputSingleAxis : IInputAxis
    {
        public string Name { get; private set; }
        public string Axis { get; private set; }
        public float Delta { get; private set; }

        public bool Invert { get; set; }

        public InputSingleAxis(string name, string axis)
        {
            Name = name;
            Axis = axis;
        }

        public void Update()
        {
            var delta = Input.GetAxis(Axis);

            if (Invert) delta = delta * -1;

            Delta = delta;
        }
    }

    /// <summary>
    /// Combines two axes (one horizontal and one vertical).
    /// </summary>
    public class InputTwinAxes : IInputAxis
    {
        public string Name { get; private set; }
        public string XAxis { get; private set; }
        public string YAxis { get; private set; }
        public Vector2 Delta { get; private set; }

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public InputTwinAxes(string name, string xAxis, string yAxis)
        {
            Name = name;
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public void Update()
        {
            var x = Input.GetAxis(XAxis);
            var y = Input.GetAxis(YAxis);

            if (InvertX) x = x * -1;
            if (InvertY) y = y * -1;

            Delta = new Vector2(x, y);

            // Trigger here?
        }
    }
}
