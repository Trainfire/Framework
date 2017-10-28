using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public class NodePinView : View
    {
        public NodePin Pin { get; private set; }
        public Rect LocalRect { get; private set; }

        public bool IsConnected
        {
            get
            {
                return Pin.ConnectedPin != null;
            }
        }

        public Vector2 ScreenPosition
        {
            get
            {
                var screenPos = LocalRect.position + Pin.Node.Position;

                if (_rightAligned)
                {
                    screenPos.x += 50f;
                }
                else
                {
                    screenPos.x += -LocalRect.width * 0.5f;
                }

                screenPos.y += LocalRect.height * 0.5f;

                return screenPos;
            }
        }

        private bool _rightAligned;

        public NodePinView(NodePin nodePin, bool rightAligned = false) : base()
        {
            Pin = nodePin;
            LocalRect = new Rect();
            _rightAligned = rightAligned;
        }

        protected override void OnDraw()
        {
            if (Pin == null)
                return;

            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (_rightAligned)
                GUILayout.FlexibleSpace();

            GUILayout.Box(Pin.Name);

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                LocalRect = GUILayoutUtility.GetLastRect();

            GUILayout.EndHorizontal();
        }
    }
}
