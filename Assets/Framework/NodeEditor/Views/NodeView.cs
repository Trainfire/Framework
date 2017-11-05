using UnityEngine;
using UnityEngine.Assertions;
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
        public List<NodePinView> PinViews
        {
            get
            {
                return _pinViews.Values.ToList();
            }
        }

        private int _windowId;
        private Rect _rect;
        private EditorInputListener _inputListener;
        private Dictionary<NodePin, NodePinView> _pinViews;

        private readonly Vector2 NodeSize;

        public NodeView(Node node, int windowId)
        {
            NodeSize = new Vector2(100f, 100f);

            _pinViews = new Dictionary<NodePin, NodePinView>();

            _inputListener = new EditorInputListener();
            _inputListener.MouseDown += InputListener_MouseLeftClicked;
            _inputListener.DeletePressed += InputListener_DeletePressed;

            Node = node;
            Node.PinAdded += Node_PinAdded;
            Node.PinRemoved += Node_PinRemoved;

            // Restore pins.
            Node.Pins.ForEach(pin => _pinViews.Add(pin, new NodePinView(pin)));

            _rect = new Rect(Node.Position, NodeSize);

            _windowId = windowId;
        }

        public void Destroy()
        {
            Node = null;
        }

        public void Draw()
        {
            if (Node == null)
                return;

            _rect = GUI.Window(_windowId, _rect, InternalDraw, Node.Name);
            Node.Position = _rect.position;
        }

        void InternalDraw(int windowId)
        {
            _inputListener.ProcessEvents();

            if (Node == null)
                return;

            // Draw.
            _pinViews
                .Values
                .ToList()
                .ForEach(x => x.Draw());

            if (GetPinUnderMouse() == null)
                GUI.DragWindow();
        }

        public NodePinView GetViewFromPin(NodePin pin)
        {
            bool hasViewForPin = _pinViews.ContainsKey(pin);
            return hasViewForPin ? _pinViews[pin] : null;
        }

        public NodePin GetPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            var view = _pinViews
                .Values
                .Where(x => x.LocalRect.Contains(_inputListener.MousePosition))
                .FirstOrDefault();

            if (view != null && view.Pin != null)
                OnPinExists.InvokeSafe(view.Pin);

            return view != null ? view.Pin : null;
        }

        void InputListener_MouseLeftClicked(EditorMouseEvent mouseEvent)
        {
            NodeSelected.InvokeSafe(this);
        }

        void InputListener_DeletePressed()
        {
            NodeDeleted.InvokeSafe(this);
        }

        void Node_PinRemoved(NodePin pin)
        {
            _pinViews.Remove(pin);
        }

        void Node_PinAdded(NodePin pin)
        {
            _pinViews.Add(pin, new NodePinView(pin));
        }
    }
}
