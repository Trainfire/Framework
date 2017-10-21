using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public class NodeView
    {
        private Node _node;
        private Rect _rect;

        private readonly Vector2 NodeSize;

        public NodeView(Node node)
        {
            NodeSize = new Vector2(200f, 100f);

            _node = node;
            _rect = new Rect(_node.Position, NodeSize);
        }

        public void Destroy()
        {
            _node = null;
        }

        public void Draw()
        {
            if (_node == null)
                return;

            _rect = GUI.Window(_node.ID, _rect, InternalDraw, _node.Name);
            _node.Position = _rect.position;
        }

        void InternalDraw(int windowId)
        {
            GUILayout.Label(_node.Position.ToString());
            GUILayout.Label(_node.ID.ToString());
            GUI.DragWindow();
        }
    }
}
