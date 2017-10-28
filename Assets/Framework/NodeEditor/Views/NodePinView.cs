using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public class NodePinView : View
    {
        public NodePin Pin { get; private set; }
        public Rect LocalRect { get { return Pin.LocalRect; } }

        public NodePinView(NodePin nodePin) : base()
        {
            Pin = nodePin;
        }

        protected override void OnDraw()
        {
            if (Pin == null)
                return;

            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (Pin.Node.IsOutputPin(Pin))
                GUILayout.FlexibleSpace();

            GUILayout.Box(Pin.Name);

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                Pin.LocalRect = GUILayoutUtility.GetLastRect();

            GUILayout.EndHorizontal();
        }
    }
}
