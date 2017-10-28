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
        private Dictionary<NodePin, NodePinView> _pinViews;

        private readonly Vector2 NodeSize;

        public NodeView(Node node)
        {
            _inputListener = new EditorInputListener();
            _inputListener.MouseLeftClicked += InputListener_MouseLeftClicked;
            _inputListener.DeletePressed += InputListener_DeletePressed;

            NodeSize = new Vector2(100f, 100f);
            Node = node;

            _pinViews = new Dictionary<NodePin, NodePinView>();
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

            Node.Pins.ForEach(x =>
            {
                if (!_pinViews.ContainsKey(x))
                    _pinViews.Add(x, new NodePinView(x, x.Node.IsOutputPin(x)));
            });

            _pinViews
                .Values
                .ToList()
                .ForEach(x => x.Draw());

            if (GetPinUnderMouse() == null)
                GUI.DragWindow();
        }

        public NodePin GetPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            var pinView = _pinViews
                .Values
                .Where(x => x.Rect.Contains(_inputListener.MousePosition))
                .FirstOrDefault();

            if (pinView != null && pinView.Pin != null)
                OnPinExists.InvokeSafe(pinView.Pin);

            return pinView == null ? null : pinView.Pin;
        }

        void InputListener_MouseLeftClicked(EditorMouseEvent mouseEvent)
        {
            NodeSelected.InvokeSafe(this);
        }

        void InputListener_DeletePressed()
        {
            NodeDeleted.InvokeSafe(this);
        }
    }
}
