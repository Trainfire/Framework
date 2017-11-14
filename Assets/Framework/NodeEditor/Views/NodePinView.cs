using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodePinView : BaseView
    {
        public enum DrawType
        {
            Input,
            Output,
        }

        public NodePin Pin { get; private set; }
        public Rect LocalRect { get { return Pin.LocalRect; } }

        private const float PinSize = 10f;

        private bool _isInputPin;
        private int _pinIndex;

        public NodePinView(NodePin nodePin) : base()
        {
            Pin = nodePin;

            _isInputPin = nodePin.Node.IsInputPin(Pin);
            _pinIndex = _isInputPin ? Pin.Node.InputPins.IndexOf(Pin) : Pin.Node.OutputPins.IndexOf(Pin);
        }

        protected override void OnDraw()
        {
            if (Pin == null)
                return;

            var startingBg = GUI.backgroundColor;

            GUI.backgroundColor = LocalRect.Contains(InputListener.MousePosition) ? Color.gray : startingBg;

            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (!_isInputPin)
                GUILayout.FlexibleSpace();

            if (_isInputPin)
            {
                DrawPin();
                DrawLabel();
            }
            else
            {
                DrawLabel();
                DrawPin();
            }

            GUILayout.EndHorizontal();

            GUI.backgroundColor = startingBg;
        }

        void DrawPin()
        {
            GUILayout.Box("", GUILayout.Width(PinSize), GUILayout.Height(PinSize));

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                Pin.LocalRect = GUILayoutUtility.GetLastRect();
        }

        void DrawLabel()
        {
            GUILayout.Label(new GUIContent(Pin.Name, Pin.ToString()));
        }

        protected override void OnDestroy()
        {
            Pin = null;
        }
    }
}
