using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodePinView : BaseView
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

            var startingBg = GUI.backgroundColor;

            GUI.backgroundColor = LocalRect.Contains(InputListener.MousePosition) ? Color.gray : startingBg;

            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (Pin.Node.IsOutputPin(Pin))
                GUILayout.FlexibleSpace();

            GUILayout.Box(new GUIContent(Pin.Name, Pin.ToString()));

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                Pin.LocalRect = GUILayoutUtility.GetLastRect();

            GUILayout.EndHorizontal();

            GUI.backgroundColor = startingBg;
        }

        protected override void OnDestroy()
        {
            Pin = null;
        }
    }
}
