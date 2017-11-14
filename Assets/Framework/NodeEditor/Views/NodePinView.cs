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

        public NodePinView(NodePin nodePin) : base()
        {
            Pin = nodePin;

            _isInputPin = nodePin.Node.IsInputPin(Pin);
        }

        protected override void OnDraw()
        {
            if (Pin == null)
                return;

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
        }

        void DrawPin()
        {
            var startingBg = GUI.backgroundColor;

            var color = NodeEditorHelper.GetPinColor(Pin.Type);

            // Highlight
            if (LocalRect.Contains(InputListener.MousePosition))
            {
                var highlightAdd = 0.4f;
                color = new Color(color.r + highlightAdd, color.g + highlightAdd, color.b + highlightAdd);
            }

            GUI.backgroundColor = color;

            GUILayout.Box("", GUILayout.Width(PinSize), GUILayout.Height(PinSize));

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                Pin.LocalRect = GUILayoutUtility.GetLastRect();

            GUI.backgroundColor = startingBg;
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
