using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    public class NodeView
    {
        public event Action<NodeView> NodeSelected;
        public event Action<NodeView> NodeDeleted;

        public Node Node { get; private set; }

        private Rect _rect;
        private EditorInputListener _inputListener;
        private List<NodePinView> _pinViews;

        private readonly Vector2 NodeSize;

        public NodeView(Node node)
        {
            _inputListener = new EditorInputListener();
            _inputListener.DeletePressed += () => NodeDeleted.InvokeSafe(this);

            NodeSize = new Vector2(150f, 200f);

            Node = node;

            _pinViews = new List<NodePinView>();
            Node.Pins.ForEach(x =>
            {
                var view = new NodePinView(x);
                _pinViews.Add(view);
            });

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

            _pinViews.ForEach(x => x.Draw());

            if (GetPinUnderMouse() == null)
                GUI.DragWindow();
        }

        public NodePin GetPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            var pinView = _pinViews
                .Where(x => x.Rect.Contains(_inputListener.MousePosition))
                .FirstOrDefault();

            if (pinView != null && pinView.Pin != null)
                OnPinExists.InvokeSafe(pinView.Pin);

            return pinView == null ? null : pinView.Pin;
        }
    }
}
