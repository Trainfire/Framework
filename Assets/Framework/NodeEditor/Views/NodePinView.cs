using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public class NodePinView : View
    {
        public NodePin Pin { get; private set; }
        public Rect Rect { get; private set; }

        private bool _rightAligned;

        public NodePinView(NodePin nodePin, bool rightAligned = false) : base()
        {
            Pin = nodePin;
            Rect = new Rect(Vector2.zero, Vector2.zero);
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

            GUILayout.EndHorizontal();

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                Rect = GUILayoutUtility.GetLastRect();
        }
    }
}
