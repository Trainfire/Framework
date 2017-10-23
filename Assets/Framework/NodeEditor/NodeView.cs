using UnityEngine;
using UnityEditor;
using System;

namespace Framework.NodeEditor
{
    public class NodeView
    {
        public event Action<NodeView> NodeSelected;
        public event Action<NodeView> NodeDeleted;

        public Node Node { get; private set; }

        private Rect _rect;
        private EditorInputListener _inputListener;

        private readonly Vector2 NodeSize;

        public NodeView(Node node)
        {
            _inputListener = new EditorInputListener();
            _inputListener.MouseLeftClicked += (mouseEvent) => NodeSelected.InvokeSafe(this);
            _inputListener.DeletePressed += () => NodeDeleted.InvokeSafe(this);

            NodeSize = new Vector2(150f, 200f);

            Node = node;
            _rect = new Rect(Node.Position, NodeSize);
        }

        public void Destroy()
        {
            Node = null;
        }

        public void Draw()
        {
            if (Node == null)
                return;

            _rect = GUI.Window(Node.ID, _rect, InternalDraw, Node.Name);
            Node.Position = _rect.position;
        }

        void InternalDraw(int windowId)
        {
            _inputListener.ProcessEvents();

            if (Node == null)
                return;

            GUILayout.Label(Node.Position.ToString());
            GUILayout.Label(Node.ID.ToString());

            GUILayout.Label("Inputs:");
            Node.InputPins.ForEach(x =>
            {
                GUILayout.Label(x.Name);   
            });

            GUILayout.Label("Outputs:");
            Node.OutputPins.ForEach(x =>
            {
                GUILayout.Label(x.Name);
            });

            GUI.DragWindow();
        }

        bool ContainsPoint(Rect rect, Vector2 point)
        {
            return point.x > 0 && point.x < rect.width && point.y > 0 && point.y < rect.height;
        }
    }
}
